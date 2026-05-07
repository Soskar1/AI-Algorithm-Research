using AiAlgorithmsResearch.Core.Combat.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class WaitActionHandler : ICombatActionHandler
    {
        public CombatActionId ActionId => CombatActionIds.Wait;
        private readonly ICombatLogger _combatLogger;

        public WaitActionHandler(ICombatLogger combatLogger)
        {
            _combatLogger = combatLogger;
        }

        public bool CanExecute(ICombatAction action, ICombatStateView stateView) => true;

        public bool Apply(ICombatAction action, ICombatStateView stateView, ICombatStateEditor stateEditor)
        {
            _combatLogger.Log(action.ExecutorId, $"skipped turn.", stateView);

            return true;
        }
    }
}
