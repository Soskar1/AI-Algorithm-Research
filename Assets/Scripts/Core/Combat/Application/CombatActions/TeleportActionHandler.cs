using AiAlgorithmsResearch.Core.Combat.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class TeleportActionHandler : ICombatActionHandler
    {
        private readonly ICombatStateView _stateView;
        private readonly ICombatStateEditor _stateEditor;
        private readonly ICombatLogger _combatLogger;

        public CombatActionId ActionId => CombatActionIds.Teleport;

        public TeleportActionHandler(ICombatStateView stateView, ICombatStateEditor stateEditor, ICombatLogger combatLogger)
        {
            _stateView = stateView;
            _stateEditor = stateEditor;
            _combatLogger = combatLogger;
        }

        public bool CanExecute(ICombatAction action)
        {
            var teleport = (TeleportAction)action;
            return _stateView.TryGetPosition(teleport.Actor.Id, out _);
        }

        public bool Apply(ICombatAction action)
        {
            var teleport = (TeleportAction)action;

            var currentPositionResult = _stateView.TryGetPosition(teleport.Actor.Id, out var currentPosition);

            var teleportResult = _stateEditor.TryMove(teleport.Actor.Id, teleport.TargetPosition);

            if (teleportResult)
            {
                var fromString = currentPositionResult ? $"from ({currentPosition})" : "";
                _combatLogger.Log(action.Actor, $"is teleported {fromString} to {teleport.TargetPosition}.");
            }
            else
            {
                _combatLogger.Log(action.Actor, $"failed to teleport to {teleport.TargetPosition}");
            }

            return teleportResult;
        }
    }
}
