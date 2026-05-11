using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
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
            var minCost = int.MaxValue;
            Vector2Int moveTarget = enemyPosition;
            
            foreach (var targetTile in Targeting.GetAdjacentTiles(enemyPosition))
            {
                var path = AStarPathfinder.FindPath(combatState, executorPosition, targetTile);

                if (path == null)
                {
                    continue;
                }

                path = path
                    .Skip(1)
                    .ToList();

                var maxSteps = path.Count - 1;
                var stepsToMake = Mathf.Min(energy * 2, maxSteps);
                var cost = Mathf.CeilToInt(stepsToMake / 2f);

                if (cost < minCost)
                {
                    minCost = cost;
                    moveTarget = path[stepsToMake];
                }
            }

            if (moveTarget == enemyPosition)
            {
                return Enumerable.Empty<ICombatAction>();
            }

            return new List<ICombatAction>()
            {
                new MoveAction(executorId, moveTarget, minCost)
            };
        }
    }
}
