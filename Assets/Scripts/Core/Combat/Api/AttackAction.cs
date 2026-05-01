using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct AttackAction : ICombatAction
    {
        public IEntityView Actor { get; }
        public CombatActionId Id => CombatActionIds.Attack;
        public IEntityView Target { get; }
        public int BaseDamage { get; }
        public int Range { get; }

        public AttackAction(IEntityView actor, IEntityView target, int baseDamage, int range)
        {
            Actor = actor;
            Target = target;
            BaseDamage = baseDamage;
            Range = range;
        }
    }
}