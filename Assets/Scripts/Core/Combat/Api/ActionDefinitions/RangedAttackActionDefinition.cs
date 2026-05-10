namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public class RangedAttackActionDefinition : ICombatActionDefinition
    {
        public CombatActionId Id => CombatActionIds.RangedAttack;
        public int BaseDamage { get; }
        public int Range { get; }
        public int BaseCost { get; }
        public int Cooldown { get; }

        public RangedAttackActionDefinition(int baseDamage, int range, int baseCost, int cooldown)
        {
            BaseDamage = baseDamage;
            Range = range;
            BaseCost = baseCost;
            Cooldown = cooldown;
        }
    }
}
