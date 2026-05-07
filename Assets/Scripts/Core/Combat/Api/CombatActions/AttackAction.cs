using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct AttackAction : ICombatAction
    {
        public EntityId ExecutorId { get; }
        public CombatActionId Id => CombatActionIds.Attack;
        public EntityId Target { get; }
        public int BaseDamage { get; }
        public int Range { get; }
        public int Cost { get; }
        public int Cooldown => 0;

        public AttackAction(EntityId actor, EntityId target, int baseDamage, int range, int cost)
        {
            ExecutorId = actor;
            Target = target;
            BaseDamage = baseDamage;
            Range = range;
            Cost = cost;
        }
    }
}