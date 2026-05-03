namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public sealed class HealActionDefinition : ICombatActionDefinition
    {
        public CombatActionId Id => CombatActionIds.Heal;

        public int Amount { get; }

        public HealActionDefinition(int amount)
        {
            Amount = amount;
        }
    }
}
