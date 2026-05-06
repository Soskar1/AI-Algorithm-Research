using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class TeleportActionHandler : ICombatActionHandler
    {
        private readonly IWorldView _worldView;
        private readonly IWorldEditor _worldEditor;
        private readonly ICombatLogger _combatLogger;

        public CombatActionId ActionId => CombatActionIds.Teleport;

        public TeleportActionHandler(IWorldView worldView, IWorldEditor worldEditor, ICombatLogger combatLogger)
        {
            _worldView = worldView;
            _worldEditor = worldEditor;
            _combatLogger = combatLogger;
        }

        public bool CanExecute(ICombatAction action)
        {
            var teleport = (TeleportAction)action;
            return _worldView.TryGetEntityPosition(teleport.Actor, out _);
        }

        public bool Apply(ICombatAction action)
        {
            var teleport = (TeleportAction)action;

            var currentPositionResult = _worldView.TryGetEntityPosition(teleport.Actor, out var currentPosition);

            var teleportResult = _worldEditor.TryMoveEntity(teleport.Actor, teleport.TargetPosition);

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
