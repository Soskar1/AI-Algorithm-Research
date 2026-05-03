using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal interface ICombatActionCandidateGenerator
    {
        CombatActionId ActionId { get; }
        IEnumerable<ICombatAction> GetCandidates(ICombatActionDefinition definition, CombatAgentContext context);
    }
}
