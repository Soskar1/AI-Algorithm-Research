using UnityEngine;
using EntityId = AiAlgorithmsResearch.Core.Entities.Api.EntityId;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface ICombatStateView
    {
        bool TryGetPosition(EntityId entityId, out Vector2Int position);
        bool IsOccupied(Vector2Int position);
        bool IsObstacle(Vector2Int position);

        int GetHealth(EntityId entityId);
        int GetMaxHealth(EntityId entityId);
        int GetEnergy(EntityId entityId);
        int GetStrength(EntityId entityId);

        bool IsOnCooldown(EntityId entityId, CombatActionId actionId);
    }
}
