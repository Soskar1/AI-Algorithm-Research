using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EntityId = AiAlgorithmsResearch.Core.Entities.Api.EntityId;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class MoveActionCandidateGenerator : ICombatActionCandidateGenerator
    {
        public CombatActionId ActionId => CombatActionIds.Move;

        public IEnumerable<ICombatAction> GetCandidates(ICombatActionDefinition definition, ICombatStateView combatState, EntityId executorId)
        {
            if (!combatState.TryGetPosition(executorId, out Vector2Int executorPosition))
            {
                return Enumerable.Empty<ICombatAction>();
            }

            if (!Targeting.TryGetClosestEnemyPosition(combatState, executorId, out var enemyPosition))
            {
                return Enumerable.Empty<ICombatAction>();
            }

            if (!Targeting.TryGetClosestValidAdjacentTileToTarget(combatState, executorId, enemyPosition, out var moveTarget))
            {
                return Enumerable.Empty<ICombatAction>();
            }

            var distance = GridDistance.Manhattan(executorPosition, moveTarget);
            var cost = Mathf.CeilToInt(distance / 2f);

            return new List<ICombatAction>()
            {
                new MoveAction(executorId, moveTarget, cost)
            };
        }
    }
}
