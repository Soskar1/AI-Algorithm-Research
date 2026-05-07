using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct StunAction : ICombatAction
    {
        public CombatActionId Id => CombatActionIds.Stun;
        public EntityId ExecutorId { get; }
        public EntityId TargetId { get; }
        public int Cost { get; }
        public int Cooldown { get; }

        public StunAction(EntityId executorId, EntityId targetId, int cost, int cooldown)
        {
            ExecutorId = executorId;
            TargetId = targetId;
            Cost = cost;
            Cooldown = cooldown;
        }
    }
}