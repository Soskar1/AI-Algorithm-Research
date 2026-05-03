using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;
using System.Linq;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class AttackActionCandidateGenerator : ICombatActionCandidateGenerator
    {
        public CombatActionId ActionId => CombatActionIds.Attack;

        public IEnumerable<ICombatAction> GetCandidates(ICombatActionDefinition definition, CombatAgentContext context)
        {
            var attack = (AttackActionDefinition)definition;

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

                if (distance > attack.Range)
                    continue;

                candidates.Add(new AttackAction(context.Actor, participant.Entity, attack.BaseDamage, attack.Range));
            }

            return candidates;
        }
    }
}
