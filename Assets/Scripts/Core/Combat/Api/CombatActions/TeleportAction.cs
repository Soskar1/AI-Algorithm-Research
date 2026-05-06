using AiAlgorithmsResearch.Core.Entities.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct TeleportAction : ICombatAction
    {
        public IEntityView Actor { get; }
        public CombatActionId Id => CombatActionIds.Teleport;
        public Vector2Int TargetPosition { get; }
        public int Cost { get; }
        public int Cooldown { get; }

        public TeleportAction(IEntityView actor, Vector2Int targetPosition, int cost, int cooldown)
        {
            Actor = actor;
            TargetPosition = targetPosition;
            Cost = cost;
            Cooldown = cooldown;
        }
    }
}