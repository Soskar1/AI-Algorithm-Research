namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public class AttackActionDefinition : ICombatActionDefinition
    {
        public CombatActionId Id => CombatActionIds.Attack;

        public int BaseDamage { get; }
        public int Range { get; }
        public int BaseCost { get; }
        public int Cooldown => 0;

        public AttackActionDefinition(int baseDamage, int range, int baseCost)
        {
            BaseDamage = baseDamage;
            Range = range;
            BaseCost = baseCost;
        }
    }
}
