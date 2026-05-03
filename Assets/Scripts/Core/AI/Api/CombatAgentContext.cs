using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Api
{
    public sealed class CombatAgentContext
    {
        public IEntityView Actor { get; }
        public IReadOnlyCollection<ICombatActionDefinition> AvailableActions { get; }
        public IWorldView World { get; }
        public IBattle Battle { get; }
        public TeamId TeamId { get; }

        public CombatAgentContext(IEntityView actor, IReadOnlyCollection<ICombatActionDefinition> availableActions, IWorldView worldView, IBattle battle, TeamId teamId)
        {
            Actor = actor;
            AvailableActions = availableActions;
            World = worldView;
            Battle = battle;
            TeamId = teamId;
        }
    }
}