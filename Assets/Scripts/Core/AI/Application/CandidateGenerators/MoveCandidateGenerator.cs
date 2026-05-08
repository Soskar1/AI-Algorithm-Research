using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System;
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
            var cost = Int32.MaxValue;
            var distance = Int32.MaxValue;
            Vector2Int moveTarget = enemyPosition;

            while (cost > energy && distance > 0)
            {
                if (!Targeting.TryGetClosestValidAdjacentTileToTarget(combatState, executorId, moveTarget, out moveTarget))
                {
                    return Enumerable.Empty<ICombatAction>();
                }

                distance = GridDistance.Manhattan(executorPosition, moveTarget);
                cost = Mathf.CeilToInt(distance / 2f);
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
