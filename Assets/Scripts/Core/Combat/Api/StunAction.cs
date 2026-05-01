using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct StunAction : ICombatAction
    {
        public IEntityView Actor { get; }
        public CombatActionId Id => CombatActionIds.Stun;

        public IEntityView Target { get; }

        public StunAction(IEntityView actor, IEntityView target)
        {
            Actor = actor;
            Target = target;
        }
    }
}