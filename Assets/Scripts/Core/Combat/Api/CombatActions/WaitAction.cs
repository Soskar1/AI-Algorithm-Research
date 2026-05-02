using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct WaitAction : ICombatAction
    {
        public IEntityView Actor { get; }
        public CombatActionId Id => CombatActionIds.Wait;

        public WaitAction(IEntityView actor)
        {
            Actor = actor;
        }
    }
}