using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class CombatActionExecutor : ICombatActionExecutor
    {
        private readonly IWorldView _worldView;
        private readonly IWorldEditor _worldEditor;
        private readonly IEntityHealthEditor _healthEditor;
        private readonly IEntityEnergyEditor _energyEditor;
        private readonly ICombatActionCostCalculator _costCalculator;
        private readonly IActionCooldowns _cooldowns;
        private readonly IActionCooldownEditor _cooldownEditor;
        private readonly IStunStatusEditor _stunStatusEditor;

        public CombatActionExecutor(
            IWorldView worldView,
            IWorldEditor worldEditor,
            IEntityHealthEditor healthEditor,
            IEntityEnergyEditor energyEditor,
            ICombatActionCostCalculator costCalculator,
            IActionCooldowns cooldowns,
            IActionCooldownEditor cooldownEditor,
            IStunStatusEditor stunStatusEditor)
        {
            _worldView = worldView;
            _worldEditor = worldEditor;
            _healthEditor = healthEditor;
            _energyEditor = energyEditor;
            _costCalculator = costCalculator;
            _cooldowns = cooldowns;
            _cooldownEditor = cooldownEditor;
            _stunStatusEditor = stunStatusEditor;
        }

        public bool TryExecute(ICombatAction action)
        {
            if (!CanExecute(action))
                return false;

            var cost = _costCalculator.GetCost(action);

            if (!_energyEditor.TrySpendEnergy(action.Actor, cost))
                return false;

            return Apply(action);
        }

        private bool CanExecute(ICombatAction action)
        {
            if (_cooldowns.IsOnCooldown(action.Actor, action.Id))
                return false;

            return action switch
            {
                WaitAction => true,
                MoveAction moveAction => CanExecuteMove(moveAction),
                AttackAction attackAction => CanExecuteAttack(attackAction),
                TeleportAction teleportAction => CanExecuteTeleport(teleportAction),
                HealAction healAction => CanExecuteHeal(healAction),
                StunAction stunAction => CanExecuteStun(stunAction),
                _ => false
            };
        }

        private bool Apply(ICombatAction action)
        {
            return action switch
            {
                WaitAction => true,
                MoveAction moveAction => ApplyMove(moveAction),
                AttackAction attackAction => ApplyAttack(attackAction),
                TeleportAction teleportAction => ApplyTeleport(teleportAction),
                HealAction healAction => ApplyHeal(healAction),
                StunAction stunAction => ApplyStun(stunAction),
                _ => false
            };
        }

        private bool CanExecuteMove(MoveAction action)
        {
            return _worldView.TryGetEntityPosition(action.Actor, out _);
        }

        private bool CanExecuteAttack(AttackAction action)
        {
            if (!_worldView.TryGetEntityPosition(action.Actor, out var actorPosition))
                return false;

            if (!_worldView.TryGetEntityPosition(action.Target, out var targetPosition))
                return false;

            var distance = GridDistance.Manhattan(actorPosition, targetPosition);

            return distance <= action.Range;
        }

        private bool CanExecuteTeleport(TeleportAction action)
        {
            return _worldView.TryGetEntityPosition(action.Actor, out _);
        }

        private bool CanExecuteHeal(HealAction action)
        {
            return action.Actor.Health.Current < action.Actor.Health.Max;
        }

        private bool CanExecuteStun(StunAction action)
        {
            if (!_worldView.TryGetEntityPosition(action.Actor, out var actorPosition))
                return false;

            if (!_worldView.TryGetEntityPosition(action.Target, out var targetPosition))
                return false;

            var distance = GridDistance.Manhattan(actorPosition, targetPosition);

            return distance == 1;
        }

        private bool ApplyMove(MoveAction action)
        {
            return _worldEditor.TryMoveEntity(action.Actor, action.TargetPosition);
        }

        private bool ApplyAttack(AttackAction action)
        {
            var damage = action.BaseDamage + action.Actor.Strength;

            _healthEditor.DealDamage(action.Target, damage);

            return true;
        }

        private bool ApplyTeleport(TeleportAction action)
        {
            var moved = _worldEditor.TryMoveEntity(
                action.Actor,
                action.TargetPosition);

            if (!moved)
                return false;

            _cooldownEditor.PutOnCooldown(action.Actor, CombatActionIds.Teleport, turns: 4);

            return true;
        }

        private bool ApplyHeal(HealAction action)
        {
            _healthEditor.Heal(action.Actor, action.Amount);

            _cooldownEditor.PutOnCooldown(action.Actor, CombatActionIds.Heal, turns: 3);

            return true;
        }

        private bool ApplyStun(StunAction action)
        {
            _stunStatusEditor.StunForNextTurn(action.Target);

            _cooldownEditor.PutOnCooldown(action.Actor, CombatActionIds.Stun, turns: 3);

            return true;
        }
    }
}