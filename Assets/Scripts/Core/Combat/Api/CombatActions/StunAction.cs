using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct StunAction : ICombatAction
    {
        public IEntityView Actor { get; }
        public CombatActionId Id => CombatActionIds.Stun;
        public IEntityView Target { get; }
        public int Cost { get; }
        public int Cooldown { get; }

        public StunAction(IEntityView actor, IEntityView target, int cost, int cooldown)
        {
            Actor = actor;
            Target = target;
            Cost = cost;
            Cooldown = cooldown;
        }
    }
}