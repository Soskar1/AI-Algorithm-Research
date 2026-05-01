using System;
using System.Linq;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class CombatActionCostCalculator : ICombatActionCostCalculator
    {
        private readonly IWorldView _worldView;

        public CombatActionCostCalculator(IWorldView worldView)
        {
            _worldView = worldView;
        }

        public int GetCost(ICombatAction action)
        {
            return action switch
            {
                WaitAction => 0,
                AttackAction => 2,
                MoveAction moveAction => GetMoveCost(moveAction),
                _ => throw new ArgumentOutOfRangeException(nameof(action))
            };
        }

        private int GetMoveCost(MoveAction action)
        {
            var worldEntity = _worldView.Entities.FirstOrDefault(entity => ReferenceEquals(entity.Entity, action.Actor));

            if (worldEntity.Entity == null)
                return int.MaxValue;

            var distance = GetManhattanDistance(worldEntity.Position, action.TargetPosition);

            return Mathf.CeilToInt(distance / 2f);
        }

        private static int GetManhattanDistance(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }
}