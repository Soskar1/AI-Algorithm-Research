namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public sealed class HealActionDefinition : ICombatActionDefinition
    {
        public CombatActionId Id => CombatActionIds.Heal;

        public int Amount { get; }
        public int BaseCost { get; }
        public int Cooldown { get; }

        public HealActionDefinition(int amount, int baseCost, int cooldown)
        {
            Amount = amount;
            BaseCost = baseCost;
            Cooldown = cooldown;
        }
    }
}
