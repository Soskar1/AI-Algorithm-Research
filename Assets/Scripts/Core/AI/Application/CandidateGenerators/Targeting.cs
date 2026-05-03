using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.AI.Application
{
    internal static class Targeting
    {
        public static bool TryGetClosestEnemyPosition(CombatAgentContext context, Vector2Int actorPosition, out Vector2Int enemyPosition)
        {
            enemyPosition = default;
            var bestDistance = int.MaxValue;
            var found = false;

            foreach (var participant in context.Battle.TurnOrder)
            {
                if (participant.TeamId == context.TeamId)
                    continue;

                if (participant.Entity.Health.Current <= 0)
                    continue;

                if (!context.World.TryGetEntityPosition(participant.Entity, out var position))
                    continue;

                var distance = GridDistance.Manhattan(actorPosition, position);

                if (distance >= bestDistance)
                    continue;

                bestDistance = distance;
                enemyPosition = position;
                found = true;
            }

            return found;
        }

        public static bool TryGetClosestValidAdjacentTileToTarget(CombatAgentContext context, Vector2Int originPosition, Vector2Int targetPosition, out Vector2Int result)
        {
            result = default;

            var candidates = GetAdjacentTiles(originPosition)
                .Where(position => IsValidDestination(context, position))
                .OrderBy(position => GridDistance.Manhattan(position, targetPosition))
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

        public static bool IsValidDestination(CombatAgentContext context, Vector2Int position)
        {
            if (!context.World.Map.TryGetNode(position, out var node))
                return false;

            if (node.Type == MapNodeType.Obstacle)
                return false;

            return !context.World.Entities.Any(entity => entity.Position == position);
        }
    }
}
