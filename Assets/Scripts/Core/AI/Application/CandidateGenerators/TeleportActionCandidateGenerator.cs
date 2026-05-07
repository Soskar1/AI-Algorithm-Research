using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;
using System.Linq;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class TeleportActionCandidateGenerator : ICombatActionCandidateGenerator
    {
        public CombatActionId ActionId => CombatActionIds.Teleport;

        public IEnumerable<ICombatAction> GetCandidates(ICombatActionDefinition definition, ICombatStateView combatState, EntityId executorId)
        {
            if (!Targeting.TryGetClosestEnemyPosition(combatState, executorId, out var enemyPosition))
            {
                return Enumerable.Empty<ICombatAction>();
            }

            if (!Targeting.TryGetClosestValidAdjacentTileToTarget(combatState, executorId, enemyPosition, out var teleportTarget))
            {
                return Enumerable.Empty<ICombatAction>();
            }

            return new List<ICombatAction>()
            {
                new TeleportAction(executorId, teleportTarget, definition.BaseCost, definition.Cooldown)
            };
        }
    }
}
