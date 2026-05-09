using AiAlgorithmsResearch.Core.Combat.Api;
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

            var energy = combatState.GetEnergy(executorId);
            var cost = int.MaxValue;
            Vector2Int moveTarget = enemyPosition;
            
            foreach (var targetTile in Targeting.GetAdjacentTiles(enemyPosition))
            {
                var path = AStarPathfinder.FindPath(combatState, executorPosition, targetTile);

                if (path == null || path.Count == 2)
                {
                    continue;
                }

                path = path
                    .Skip(1)
                    .ToList();

                var maxSteps = path.Count - 1;
                var stepsToMake = Mathf.Min(energy * 2, maxSteps);
                cost = Mathf.CeilToInt(stepsToMake / 2f);
                moveTarget = path[stepsToMake];
                break;
            }

            if (moveTarget == enemyPosition)
            {
                return Enumerable.Empty<ICombatAction>();
            }

            return new List<ICombatAction>()
            {
                new MoveAction(executorId, moveTarget, cost)
            };
        }
    }
}
