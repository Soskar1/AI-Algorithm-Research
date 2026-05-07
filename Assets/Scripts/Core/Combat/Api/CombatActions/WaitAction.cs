using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct WaitAction : ICombatAction
    {
        public EntityId ExecutorId { get; }
        public CombatActionId Id => CombatActionIds.Wait;
        public int Cost => 0;
        public int Cooldown => 0;

        public WaitAction(EntityId actor)
        {
            ExecutorId = actor;
        }
    }
}