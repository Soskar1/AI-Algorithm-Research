using UnityEngine;
using EntityId = AiAlgorithmsResearch.Core.Entities.Api.EntityId;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface ICombatStateEditor
    {
        bool TryMove(EntityId entityId, Vector2Int position);
        bool TrySpendEnergy(EntityId entityId, int amount);
        void DealDamage(EntityId entityId, int amount);
        void Heal(EntityId entityId, int amount);
        void PutOnCooldown(EntityId entityId, CombatActionId actionId, int turns);
        void StunForNextTurn(EntityId entityId);
    }
}
