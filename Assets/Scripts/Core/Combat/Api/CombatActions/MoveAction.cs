using UnityEngine;
using EntityId = AiAlgorithmsResearch.Core.Entities.Api.EntityId;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct MoveAction : ICombatAction
    {
        public EntityId ExecutorId { get; }
        public CombatActionId Id => CombatActionIds.Move;
        public Vector2Int TargetPosition { get; }
        public int Cost { get; }
        public int Cooldown => 0;

        public MoveAction(EntityId executorId, Vector2Int targetPosition, int cost)
        {
            ExecutorId = executorId;
            TargetPosition = targetPosition;
            Cost = cost;
        }
    }
}