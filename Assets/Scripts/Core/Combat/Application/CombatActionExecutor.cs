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

        public CombatActionExecutor(
            IWorldView worldView,
            IWorldEditor worldEditor,
            IEntityHealthEditor healthEditor,
            IEntityEnergyEditor energyEditor,
            ICombatActionCostCalculator costCalculator,
            IActionCooldowns cooldowns,
            IActionCooldownEditor cooldownEditor)
        {
            _worldView = worldView;
            _worldEditor = worldEditor;
            _healthEditor = healthEditor;
            _energyEditor = energyEditor;
            _costCalculator = costCalculator;
            _cooldowns = cooldowns;
            _cooldownEditor = cooldownEditor;
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

            _cooldownEditor.PutOnCooldown(
                action.Actor,
                CombatActionIds.Teleport,
                turns: 4);

            return true;
        }
    }
}