using UnityEngine;
using EntityId = AiAlgorithmsResearch.Core.Entities.Api.EntityId;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct TeleportAction : ICombatAction
    {
        public CombatActionId Id => CombatActionIds.Teleport;
        public EntityId ExecutorId { get; }
        public Vector2Int TargetPosition { get; }
        public int Cost { get; }
        public int Cooldown { get; }

        public TeleportAction(EntityId executorId, Vector2Int targetPosition, int cost, int cooldown)
        {
            ExecutorId = executorId;
            TargetPosition = targetPosition;
            Cost = cost;
            Cooldown = cooldown;
        }
    }
}