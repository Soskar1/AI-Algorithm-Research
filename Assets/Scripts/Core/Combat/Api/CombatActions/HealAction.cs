using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct HealAction : ICombatAction
    {
        public IEntityView Actor { get; }
        public CombatActionId Id => CombatActionIds.Heal;
        public int Amount { get; }
        public int Cost { get; }
        public int Cooldown { get; }

        public HealAction(IEntityView actor, int cost, int cooldown, int amount = 5)
        {
            Actor = actor;
            Amount = amount;
            Cost = cost;
            Cooldown = cooldown;
        }
    }
}