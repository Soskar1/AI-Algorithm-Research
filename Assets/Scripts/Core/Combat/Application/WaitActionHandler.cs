using AiAlgorithmsResearch.Core.Combat.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class WaitActionHandler : ICombatActionHandler
    {
        public CombatActionId ActionId => CombatActionIds.Wait;

        public bool CanExecute(ICombatAction action) => true;

        public int GetCost(ICombatAction action) => 0;

        public bool Apply(ICombatAction action) => true;

        public int GetCooldown(ICombatAction action) => 0;
    }
}
