using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.AI.Application
{
    internal sealed class HealActionCandidateGenerator : ICombatActionCandidateGenerator
    {
        public CombatActionId ActionId => CombatActionIds.Heal;

        public IEnumerable<ICombatAction> GetCandidates(ICombatActionDefinition definition, CombatAgentContext context)
        {
            var heal = (HealActionDefinition)definition;

            return new List<ICombatAction>()
            {
                new HealAction(context.Actor, heal.Amount)
            };
        }
    }
}
