namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public class AttackActionDefinition : ICombatActionDefinition
    {
        public CombatActionId Id => CombatActionIds.Attack;

        public int BaseDamage { get; }
        public int Range { get; }

        public AttackActionDefinition(int baseDamage, int range)
        {
            BaseDamage = baseDamage;
            Range = range;
        }
    }
}
