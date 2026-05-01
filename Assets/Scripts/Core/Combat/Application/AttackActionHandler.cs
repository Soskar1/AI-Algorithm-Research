using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class AttackActionHandler : ICombatActionHandler
    {
        private readonly IWorldView _worldView;
        private readonly IEntityHealthEditor _healthEditor;

        public CombatActionId ActionId => CombatActionIds.Attack;

        public AttackActionHandler(IWorldView worldView, IEntityHealthEditor healthEditor)
        {
            _worldView = worldView;
            _healthEditor = healthEditor;
        }

        public bool CanExecute(ICombatAction action)
        {
            var attack = (AttackAction)action;

            if (!_worldView.TryGetEntityPosition(attack.Actor, out var actorPosition))
                return false;

            if (!_worldView.TryGetEntityPosition(attack.Target, out var targetPosition))
                return false;

            return GridDistance.Manhattan(actorPosition, targetPosition) <= attack.Range;
        }

        public int GetCost(ICombatAction action)
        {
            return 2;
        }

        public bool Apply(ICombatAction action)
        {
            var attack = (AttackAction)action;
            var damage = attack.BaseDamage + attack.Actor.Strength;

            _healthEditor.DealDamage(attack.Target, damage);

            return true;
        }

        public int GetCooldown(ICombatAction action)
        {
            return 0;
        }
    }
}