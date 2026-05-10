using AiAlgorithmsResearch.Core.Combat.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal class RangedAttackActionHandler : ICombatActionHandler
    {
        private readonly ICombatLogger _combatLogger;

        public RangedAttackActionHandler(ICombatLogger combatLogger)
        {
            _combatLogger = combatLogger;
        }

        public bool CanExecute(ICombatAction action, ICombatStateView stateView)
        {
            var attack = (RangedAttackAction)action;

            if (!stateView.TryGetPosition(attack.ExecutorId, out var actorPosition))
                return false;

            if (!stateView.TryGetPosition(attack.Target, out var targetPosition))
                return false;

            var line = BresenhamLine.DrawLine(actorPosition, targetPosition);

            foreach (var tile in line)
            {
                if (tile == actorPosition || tile == targetPosition)
                {
                    continue;
                }

                if (stateView.IsObstacle(tile) || stateView.IsOccupied(tile))
                {
                    return false;
                }
            }

            return true;
        }

        public bool Apply(ICombatAction action, ICombatStateView stateView, ICombatStateEditor stateEditor)
        {
            var attack = (RangedAttackAction)action;

            var strength = stateView.GetStrength(action.ExecutorId);
            var damage = attack.BaseDamage + strength;

            stateEditor.DealDamage(attack.Target, damage);

            var targetWithPositionLog = _combatLogger.GetEntityRepresentation(attack.Target, stateView);
            var targetId = _combatLogger.GetEntityDisplayName(attack.Target);
            var health = stateView.GetHealth(attack.Target);
            _combatLogger.Log(action.ExecutorId, $"is dealing {damage} damage with a ranged attack to {targetWithPositionLog}. {targetId} Health: {health}", stateView);

            return true;
        }
    }
}
