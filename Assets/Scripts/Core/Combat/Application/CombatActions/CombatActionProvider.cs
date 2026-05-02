using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class CombatActionProvider : ICombatActionProvider
    {
        private readonly IWorldView _worldView;
        private readonly IActionCooldowns _cooldowns;

        public CombatActionProvider(IWorldView worldView, IActionCooldowns cooldowns)
        {
            _worldView = worldView;
            _cooldowns = cooldowns;
        }

        public IReadOnlyCollection<ICombatAction> GetAvailableActions(IBattle battle, IBattleParticipant participant)
        {
            var actor = participant.Entity;
            var actions = new List<ICombatAction>
            {
                new WaitAction(actor)
            };

            // Add other actions later:
            // Attack
            // Heal
            // Teleport
            // Stun

            if (!_worldView.TryGetEntityPosition(actor, out var position))
                return actions;

            var moveActions = AddMoveActions(battle, participant, position);
            actions.AddRange(moveActions);

            return actions;
        }

        private IEnumerable<ICombatAction> AddMoveActions(IBattle battle, IBattleParticipant participant, Vector2Int position)
        {
            var actor = participant.Entity;

            var closestEnemyPosition = TryGetClosestEnemyPosition(battle, participant, position);

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

                if (!IsValidTile(target))
                    continue;

                if (IsOccupied(target))
                    continue;

                var newDistance = GridDistance.Manhattan(target, closestEnemyPosition.Value);

                if (newDistance >= currentDistance)
                    continue;

                moveActions.Add(new MoveAction(actor, target));
            }

            return moveActions;
        }

        private Vector2Int? TryGetClosestEnemyPosition(IBattle battle, IBattleParticipant participant, Vector2Int actorPosition)
        {
            Vector2Int? closest = null;
            var bestDistance = int.MaxValue;

            foreach (var other in battle.TurnOrder)
            {
                if (other.TeamId == participant.TeamId)
                    continue;

                if (!_worldView.TryGetEntityPosition(other.Entity, out var enemyPosition))
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

        private bool IsValidTile(Vector2Int position)
        {
            if (!_worldView.Map.TryGetNode(position, out var node))
                return false;

            return node.Type != MapNodeType.Obstacle;
        }

        private bool IsOccupied(Vector2Int position)
        {
            foreach (var entity in _worldView.Entities)
            {
                if (entity.Position == position)
                    return true;
            }

            return false;
        }
    }
}
