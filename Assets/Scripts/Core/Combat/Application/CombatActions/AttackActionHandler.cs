using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class AttackActionHandler : ICombatActionHandler
    {
        private readonly ICombatLogger _combatLogger;

        public CombatActionId ActionId => CombatActionIds.Attack;

        public AttackActionHandler(ICombatLogger logger)
        {
            _combatLogger = logger;
        }

        public bool CanExecute(ICombatAction action, ICombatStateView stateView)
        {
            var attack = (AttackAction)action;

            if (!stateView.TryGetPosition(attack.ExecutorId, out var actorPosition))
                return false;

            if (!stateView.TryGetPosition(attack.Target, out var targetPosition))
                return false;

            return GridDistance.Manhattan(actorPosition, targetPosition) <= attack.Range;
        }

        public bool Apply(ICombatAction action, ICombatStateView stateView, ICombatStateEditor stateEditor)
        {
            var attack = (AttackAction)action;

            var strength = stateView.GetStrength(action.ExecutorId);
            var damage = attack.BaseDamage + strength;

            stateEditor.DealDamage(attack.Target, damage);

            var targetWithPositionLog = _combatLogger.GetEntityRepresentation(attack.Target, stateView);
            var targetId = _combatLogger.GetEntityDisplayName(attack.Target);
            var health = stateView.GetHealth(attack.Target);
            _combatLogger.Log(action.ExecutorId, $"is dealing {damage} damage to {targetWithPositionLog}. {targetId} Health: {health}", stateView);

            return true;
        }
    }
}