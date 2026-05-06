using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class MoveActionHandler : ICombatActionHandler
    {
        private readonly IWorldView _worldView;
        private readonly IWorldEditor _worldEditor;
        private readonly ICombatLogger _combatLogger;

        public CombatActionId ActionId => CombatActionIds.Move;

        public MoveActionHandler(IWorldView worldView, IWorldEditor worldEditor, ICombatLogger combatLogger)
        {
            _worldView = worldView;
            _worldEditor = worldEditor;
            _combatLogger = combatLogger;
        }

        public bool CanExecute(ICombatAction action)
        {
            var move = (MoveAction)action;
            return _worldView.TryGetEntityPosition(move.Actor, out _);
        }

        public bool Apply(ICombatAction action)
        {
            var move = (MoveAction)action;

            var currentPositionResult = _worldView.TryGetEntityPosition(move.Actor, out var currentPosition);

            var movedEntity = _worldEditor.TryMoveEntity(move.Actor, move.TargetPosition);

            if (movedEntity)
            {
                var fromString = currentPositionResult ? $"from ({currentPosition})" : "";
                _combatLogger.Log(action.Actor, $"is moved {fromString} to {move.TargetPosition}.");
            }
            else
            {
                _combatLogger.Log(action.Actor, $"failed to moved to {move.TargetPosition}");
            }

            return movedEntity;
        }

        public int GetCooldown(ICombatAction action)
        {
            return 0;
        }
    }
}
