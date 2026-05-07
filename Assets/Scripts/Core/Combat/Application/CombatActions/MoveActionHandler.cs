using AiAlgorithmsResearch.Core.Combat.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class MoveActionHandler : ICombatActionHandler
    {
        public CombatActionId ActionId => CombatActionIds.Move;

        private readonly ICombatLogger _combatLogger;

        public MoveActionHandler(ICombatLogger combatLogger)
        {
            _combatLogger = combatLogger;
        }

        public bool CanExecute(ICombatAction action, ICombatStateView stateView)
        {
            var move = (MoveAction)action;
            return stateView.TryGetPosition(move.ExecutorId, out _);
        }

        public bool Apply(ICombatAction action, ICombatStateView stateView, ICombatStateEditor stateEditor)
        {
            var move = (MoveAction)action;

            var currentPositionResult = stateView.TryGetPosition(move.ExecutorId, out var currentPosition);

            var movedEntity = stateEditor.TryMove(move.ExecutorId, move.TargetPosition);

            if (movedEntity)
            {
                var fromString = currentPositionResult ? $"from ({currentPosition})" : "";
                _combatLogger.Log(action.ExecutorId, $"is moved {fromString} to {move.TargetPosition}.", stateView);
            }
            else
            {
                _combatLogger.Log(action.ExecutorId, $"failed to moved to {move.TargetPosition}", stateView);
            }

            return movedEntity;
        }
    }
}
