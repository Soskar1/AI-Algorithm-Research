using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Domain
{
    internal sealed class BattleParticipant : IBattleParticipant
    {
        public IEntityView Entity { get; }
        public int Initiative { get; }
        public TeamId TeamId { get; }
        public IReadOnlyCollection<ICombatActionDefinition> ActionDefinitions { get; }

        public BattleParticipant(IEntityView entity, int initiative, TeamId teamId, IReadOnlyCollection<ICombatActionDefinition> actionDefinitions)
        {
            Entity = entity;
            Initiative = initiative;
            TeamId = teamId;
            ActionDefinitions = actionDefinitions;
        }
    }
}
