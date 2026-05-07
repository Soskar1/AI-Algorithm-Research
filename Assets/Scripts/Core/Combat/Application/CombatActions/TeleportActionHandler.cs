using AiAlgorithmsResearch.Core.Combat.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class TeleportActionHandler : ICombatActionHandler
    {
        public CombatActionId ActionId => CombatActionIds.Teleport;
        
        private readonly ICombatLogger _combatLogger;

        public TeleportActionHandler(ICombatLogger combatLogger)
        {
            _combatLogger = combatLogger;
        }

        public bool CanExecute(ICombatAction action, ICombatStateView stateView)
        {
            var teleport = (TeleportAction)action;
            return stateView.TryGetPosition(teleport.ExecutorId, out _);
        }

        public bool Apply(ICombatAction action, ICombatStateView stateView, ICombatStateEditor stateEditor)
        {
            var teleport = (TeleportAction)action;

            var currentPositionResult = stateView.TryGetPosition(teleport.ExecutorId, out var currentPosition);

            var teleportResult = stateEditor.TryMove(teleport.ExecutorId, teleport.TargetPosition);

            if (teleportResult)
            {
                var fromString = currentPositionResult ? $"from ({currentPosition})" : "";
                _combatLogger.Log(action.ExecutorId, $"is teleported {fromString} to {teleport.TargetPosition}.", stateView);
            }
            else
            {
                _combatLogger.Log(action.ExecutorId, $"failed to teleport to {teleport.TargetPosition}", stateView);
            }

            return teleportResult;
        }
    }
}
