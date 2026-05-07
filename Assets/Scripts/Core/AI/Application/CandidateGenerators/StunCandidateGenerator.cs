using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;
using System.Linq;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class StunActionCandidateGenerator : ICombatActionCandidateGenerator
    {
        public CombatActionId ActionId => CombatActionIds.Stun;

        public IEnumerable<ICombatAction> GetCandidates(ICombatActionDefinition definition, ICombatStateView combatState, EntityId executorId)
        {
            var stun = (StunActionDefinition)definition;

            if (!combatState.TryGetPosition(executorId, out var actorPosition))
                return Enumerable.Empty<ICombatAction>();

            var candidates = new List<ICombatAction>();
            foreach (var entityId in combatState.EntityIds)
            {
                if (Targeting.AreInTheSameTeam(combatState, entityId, executorId) || Targeting.EntityIsDead(combatState, entityId))
                    continue;

                if (!combatState.TryGetPosition(entityId, out var targetPosition))
                    continue;

                var distance = GridDistance.Manhattan(actorPosition, targetPosition);

                if (distance > stun.Range)
                    continue;

                candidates.Add(new StunAction(executorId, entityId, definition.BaseCost, definition.Cooldown));
            }

            return candidates;
        }
    }
}
