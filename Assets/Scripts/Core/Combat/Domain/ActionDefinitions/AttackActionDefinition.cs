using AiAlgorithmsResearch.Core.Combat.Api;

namespace AiAlgorithmsResearch.Core.Combat.Domain
{
    internal class AttackActionDefinition : ICombatActionDefinition
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
