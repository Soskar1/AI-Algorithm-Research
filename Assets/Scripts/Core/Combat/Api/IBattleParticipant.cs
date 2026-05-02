using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface IBattleParticipant
    {
        IEntityView Entity { get; }
        TeamId TeamId { get; }
        int Initiative { get; }
        IReadOnlyCollection<ICombatActionDefinition> ActionDefinitions { get; }
    }
}
