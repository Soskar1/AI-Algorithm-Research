using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct RangedAttackAction : ICombatAction
    {
        public EntityId ExecutorId { get; }
        public CombatActionId Id => CombatActionIds.RangedAttack;
        public EntityId Target { get; }
        public int BaseDamage { get; }
        public int Range { get; }
        public int Cost { get; }
        public int Cooldown { get; }

        public RangedAttackAction(EntityId actor, EntityId target, int baseDamage, int range, int cost, int cooldown)
        {
            ExecutorId = actor;
            Target = target;
            BaseDamage = baseDamage;
            Range = range;
            Cost = cost;
            Cooldown = cooldown;
        }
    }
}