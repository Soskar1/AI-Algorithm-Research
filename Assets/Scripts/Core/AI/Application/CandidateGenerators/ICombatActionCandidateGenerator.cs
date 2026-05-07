using AiAlgorithmsResearch.Core.Ai.Domain;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal interface ICombatActionCandidateGenerator
    {
        CombatActionId ActionId { get; }
        IEnumerable<ICombatAction> GetCandidates(ICombatActionDefinition definition, ICombatStateView combatState, EntityId executorId);
    }
}
