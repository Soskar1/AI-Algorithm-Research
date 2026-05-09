using System.Collections.Generic;
using UnityEngine;
using EntityId = AiAlgorithmsResearch.Core.Entities.Api.EntityId;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface ICombatStateView
    {
        IReadOnlyCollection<EntityId> EntityIds { get; }
        EntityId CurrentEntityTurn { get; }
        IReadOnlyList<EntityId> TurnOrder { get; }

        bool TryGetPosition(EntityId entityId, out Vector2Int position);
        bool IsOccupied(Vector2Int position);
        bool IsObstacle(Vector2Int position);

        int GetHealth(EntityId entityId);
        int GetMaxHealth(EntityId entityId);
        int GetEnergy(EntityId entityId);
        int GetMaxEnergy(EntityId entityId);
        int GetEnergyRegenerationPerTurn(EntityId entityId);
        int GetStrength(EntityId entityId);
        int GetSpeed(EntityId entityId);
        IReadOnlyCollection<ICombatActionDefinition> GetCombatActionDefinitions(EntityId entityId);

        bool IsOnCooldown(EntityId entityId, CombatActionId actionId);
        bool IsStunned(EntityId entityId);

        TeamId GetTeamId(EntityId entityId);
        IDictionary<CombatActionId, int> GetCooldowns(EntityId entityId);

    }
}
