using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class StunActionCandidateGenerator : ICombatActionCandidateGenerator
    {
        public CombatActionId ActionId => CombatActionIds.Stun;

        public IEnumerable<ICombatAction> GetCandidates(ICombatActionDefinition definition, CombatAgentContext context)
        {
            var candidates = new List<ICombatAction>();

            foreach (var participant in context.Battle.TurnOrder)
            {
                if (participant.TeamId == context.TeamId)
                    continue;

                if (participant.Entity.Health.Current <= 0)
                    continue;

                candidates.Add(new StunAction(context.Actor, participant.Entity));
            }

            return candidates;
        }
    }
}
