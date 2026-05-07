using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class HealActionCandidateGenerator : ICombatActionCandidateGenerator
    {
        public CombatActionId ActionId => CombatActionIds.Heal;

        public IEnumerable<ICombatAction> GetCandidates(ICombatActionDefinition definition, ICombatStateView combatState, EntityId executorId)
        {
            var heal = (HealActionDefinition)definition;

            return new List<ICombatAction>()
            {
                new HealAction(executorId, heal.Amount, definition.Cooldown)
            };
        }
    }
}
