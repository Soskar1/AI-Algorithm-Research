using System.Collections.Generic;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Ai.Api
{
    public sealed class CombatAgentContext
    {
        public IEntityView Actor { get; }
        public IReadOnlyCollection<ICombatAction> AvailableActions { get; }

        public CombatAgentContext(IEntityView actor, IReadOnlyCollection<ICombatAction> availableActions)
        {
            Actor = actor;
            AvailableActions = availableActions;
        }
    }
}