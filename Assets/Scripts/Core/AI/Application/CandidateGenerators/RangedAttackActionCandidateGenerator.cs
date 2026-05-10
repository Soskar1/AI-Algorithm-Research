using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;
using System.Linq;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class RangedAttackActionCandidateGenerator : ICombatActionCandidateGenerator
    {
        public CombatActionId ActionId => CombatActionIds.RangedAttack;

        public IEnumerable<ICombatAction> GetCandidates(ICombatActionDefinition definition, ICombatStateView combatState, EntityId executorId)
        {
            var attack = (RangedAttackActionDefinition)definition;

            if (!combatState.TryGetPosition(executorId, out var actorPosition))
                return Enumerable.Empty<ICombatAction>();

            var executorTeamId = combatState.GetTeamId(executorId);

            var candidates = new List<ICombatAction>();
            foreach (var entity in combatState.EntityIds)
            {
                if (combatState.GetTeamId(entity) == executorTeamId)
                    continue;

                var health = combatState.GetHealth(entity);
                if (health <= 0)
                    continue;

                if (!combatState.TryGetPosition(entity, out var targetPosition))
                    continue;

                var distance = GridDistance.Manhattan(actorPosition, targetPosition);

                if (distance > attack.Range)
                    continue;

                candidates.Add(new RangedAttackAction(executorId, entity, attack.BaseDamage, attack.Range, definition.BaseCost, attack.Cooldown));
            }

            return candidates;
        }
    }
}
