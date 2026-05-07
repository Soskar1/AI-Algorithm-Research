using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct HealAction : ICombatAction
    {
        public EntityId ExecutorId { get; }
        public CombatActionId Id => CombatActionIds.Heal;
        public int Amount { get; }
        public int Cost { get; }
        public int Cooldown { get; }

        public HealAction(EntityId executorId, int cost, int cooldown, int amount = 5)
        {
            ExecutorId = executorId;
            Amount = amount;
            Cost = cost;
            Cooldown = cooldown;
        }
    }
}