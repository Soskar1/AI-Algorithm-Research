using AiAlgorithmsResearch.Core.Combat.Api;

namespace AiAlgorithmsResearch.Core.Combat.Domain
{
    internal sealed class HealActionDefinition : ICombatActionDefinition
    {
        public CombatActionId Id => CombatActionIds.Heal;

        public int Amount { get; }

        public HealActionDefinition(int amount)
        {
            Amount = amount;
        }
    }
}
