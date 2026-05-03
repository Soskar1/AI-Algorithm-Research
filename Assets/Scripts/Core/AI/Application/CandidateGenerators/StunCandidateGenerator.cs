using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;
using System.Linq;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class StunActionCandidateGenerator : ICombatActionCandidateGenerator
    {
        public CombatActionId ActionId => CombatActionIds.Stun;

        public IEnumerable<ICombatAction> GetCandidates(ICombatActionDefinition definition, CombatAgentContext context)
        {
            var stun = (StunActionDefinition)definition;

            if (!context.World.TryGetEntityPosition(context.Actor, out var actorPosition))
                return Enumerable.Empty<ICombatAction>();

            var candidates = new List<ICombatAction>();
            foreach (var participant in context.Battle.TurnOrder)
            {
                if (participant.TeamId == context.TeamId)
                    continue;

                if (participant.Entity.Health.Current <= 0)
                    continue;

                if (!context.World.TryGetEntityPosition(participant.Entity, out var targetPosition))
                    continue;

                var distance = GridDistance.Manhattan(actorPosition, targetPosition);

                if (distance > stun.Range)
                    continue;

                candidates.Add(new StunAction(context.Actor, participant.Entity));
            }

            return candidates;
        }
    }
}
