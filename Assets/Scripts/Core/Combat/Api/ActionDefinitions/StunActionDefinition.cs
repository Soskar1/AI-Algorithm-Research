namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public class StunActionDefinition : ICombatActionDefinition
    {
        public CombatActionId Id => CombatActionIds.Stun;
        public int Range { get; }

        public StunActionDefinition(int range)
        {
            Range = range;
        }
    }
}
