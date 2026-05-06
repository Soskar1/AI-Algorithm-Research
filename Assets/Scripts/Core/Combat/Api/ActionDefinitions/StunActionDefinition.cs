namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public class StunActionDefinition : ICombatActionDefinition
    {
        public CombatActionId Id => CombatActionIds.Stun;
        public int Range { get; }
        public int BaseCost { get; }
        public int Cooldown { get; }

        public StunActionDefinition(int range, int baseCost, int cooldown)
        {
            Range = range;
            BaseCost = baseCost;
            Cooldown = cooldown;
        }
    }
}
