using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EntityId = AiAlgorithmsResearch.Core.Entities.Api.EntityId;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal static class Targeting
    {
        public static bool TryGetClosestEnemyPosition(ICombatStateView combatState, EntityId executorId, out Vector2Int enemyPosition)
        {
            enemyPosition = default;
            var bestDistance = int.MaxValue;
            var found = false;

            if (!combatState.TryGetPosition(executorId, out Vector2Int executorPosition))
            {
                return false;
            }

            foreach (var entityId in combatState.EntityIds)
            {
                if (AreInTheSameTeam(combatState, entityId, executorId) || EntityIsDead(combatState, entityId))
                    continue;

                if (!combatState.TryGetPosition(entityId, out var position))
                    continue;

                var distance = GridDistance.Manhattan(executorPosition, position);

                if (distance >= bestDistance)
                    continue;

                bestDistance = distance;
                enemyPosition = position;
                found = true;
            }

            return found;
        }

        public static bool TryGetClosestValidAdjacentTileToTarget(ICombatStateView combatState, EntityId executorId, Vector2Int targetPosition, out Vector2Int result)
        {
            result = default;

            if (!combatState.TryGetPosition(executorId, out Vector2Int executorPosition))
            {
                return false;
            }

            var candidates = GetAdjacentTiles(targetPosition)
                .Where(position => IsValidDestination(combatState, position))
                .OrderBy(position => GridDistance.Manhattan(executorPosition, targetPosition))
                .ToArray();

            if (candidates.Length == 0)
                return false;

            result = candidates[0];
            return true;
        }

        private static IEnumerable<Vector2Int> GetAdjacentTiles(Vector2Int position)
        {
            yield return position + Vector2Int.right;
            yield return position + Vector2Int.left;
            yield return position + Vector2Int.up;
            yield return position + Vector2Int.down;
        }

        public static bool IsValidDestination(ICombatStateView combatState, Vector2Int position)
        {
            return !combatState.IsObstacle(position) && !combatState.IsOccupied(position);
        }

        public static bool AreInTheSameTeam(ICombatStateView combatState, EntityId first, EntityId second)
        {
            var firstTeam = combatState.GetTeamId(first);
            var secondTeam = combatState.GetTeamId(second);

            return firstTeam == secondTeam;
        }

        public static bool EntityIsDead(ICombatStateView combatState, EntityId entityId)
        {
            var health = combatState.GetHealth(entityId);
            return health <= 0;
        }
    }
}
