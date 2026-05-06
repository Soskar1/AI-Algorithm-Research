using AiAlgorithmsResearch.Core.Combat.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class MoveActionHandler : ICombatActionHandler
    {
        private readonly ICombatStateView _stateView;
        private readonly ICombatStateEditor _stateEditor;
        private readonly ICombatLogger _combatLogger;

        public CombatActionId ActionId => CombatActionIds.Move;

        public MoveActionHandler(ICombatStateView stateView, ICombatStateEditor stateEditor, ICombatLogger combatLogger)
        {
            _stateView = stateView;
            _stateEditor = stateEditor;
            _combatLogger = combatLogger;
        }

        public bool CanExecute(ICombatAction action)
        {
            var move = (MoveAction)action;
            return _stateView.TryGetPosition(move.Actor.Id, out _);
        }

        public bool Apply(ICombatAction action)
        {
            var move = (MoveAction)action;

            var currentPositionResult = _stateView.TryGetPosition(move.Actor.Id, out var currentPosition);

            var movedEntity = _stateEditor.TryMove(move.Actor.Id, move.TargetPosition);

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
    }
}
