using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class StunActionHandler : ICombatActionHandler
    {
        private readonly ICombatLogger _combatLogger;

        public CombatActionId ActionId => CombatActionIds.Stun;

        public StunActionHandler(ICombatLogger combatLogger)
        {
            _combatLogger = combatLogger;
        }

        public bool CanExecute(ICombatAction action, ICombatStateView stateView)
        {
            var stun = (StunAction)action;

            if (!stateView.TryGetPosition(stun.ExecutorId, out var actorPosition))
                return false;

            if (!stateView.TryGetPosition(stun.TargetId, out var targetPosition))
                return false;

            return GridDistance.Manhattan(actorPosition, targetPosition) == 1;
        }

        public bool Apply(ICombatAction action, ICombatStateView stateView, ICombatStateEditor stateEditor)
        {
            var stun = (StunAction)action;
            stateEditor.StunForNextTurn(stun.TargetId);

            var targetLog = _combatLogger.GetEntityRepresentation(stun.TargetId, stateView);
            _combatLogger.Log(action.ExecutorId, $"stunned {targetLog}.", stateView);

            return true;
        }
    }
}
