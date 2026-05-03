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

        public bool CanExecute(ICombatAction action) => true;

        public int GetCost(ICombatAction action) => 0;

        public bool Apply(ICombatAction action)
        {
            _combatLogger.Log(action.Actor, $"skipped turn.");

            return true;
        }

        public int GetCooldown(ICombatAction action) => 0;
    }
}
