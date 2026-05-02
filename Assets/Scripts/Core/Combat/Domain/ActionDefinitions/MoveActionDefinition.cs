using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Domain
{
    internal class MoveActionDefinition : ICombatActionDefinition
    {
        public CombatActionId Id => CombatActionIds.Move;

        public IEnumerable<ICombatAction> GetMoveAction(IWorldView worldView, IBattle battle, IBattleParticipant participant, Vector2Int position)
        {
            var actor = participant.Entity;

            var closestEnemyPosition = TryGetClosestEnemyPosition(worldView, battle, participant, position);

            if (closestEnemyPosition == null)
                return null;

            var currentDistance = GridDistance.Manhattan(position, closestEnemyPosition.Value);

            var directions = new[]
            {
                new Vector2Int(1, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, -1)
            };

            var moveActions = new List<ICombatAction>();
            foreach (var direction in directions)
            {
                var target = position + direction;

                if (!IsValidTile(worldView, target))
                    continue;

                if (IsOccupied(worldView,target))
                    continue;

                var newDistance = GridDistance.Manhattan(target, closestEnemyPosition.Value);

                if (newDistance >= currentDistance)
                    continue;

                moveActions.Add(new MoveAction(actor, target));
            }

            return moveActions;
        }

        private Vector2Int? TryGetClosestEnemyPosition(IWorldView worldView, IBattle battle, IBattleParticipant participant, Vector2Int actorPosition)
        {
            Vector2Int? closest = null;
            var bestDistance = int.MaxValue;

            foreach (var other in battle.TurnOrder)
            {
                if (other.TeamId == participant.TeamId)
                    continue;

                if (!worldView.TryGetEntityPosition(other.Entity, out var enemyPosition))
                    continue;

                var distance = GridDistance.Manhattan(actorPosition, enemyPosition);

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    closest = enemyPosition;
                }
            }

            return closest;
        }

        private bool IsValidTile(IWorldView worldView, Vector2Int position)
        {
            if (!worldView.Map.TryGetNode(position, out var node))
                return false;

            return node.Type != MapNodeType.Obstacle;
        }

        private bool IsOccupied(IWorldView worldView, Vector2Int position)
        {
            foreach (var entity in worldView.Entities)
            {
                if (entity.Position == position)
                    return true;
            }

            return false;
        }
    }
}
