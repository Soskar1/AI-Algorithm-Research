using AiAlgorithmsResearch.Core.Entities.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct TeleportAction : ICombatAction
    {
        public IEntityView Actor { get; }
        public CombatActionId Id => CombatActionIds.Teleport;
        public Vector2Int TargetPosition { get; }

        public TeleportAction(IEntityView actor, Vector2Int targetPosition)
        {
            Actor = actor;
            TargetPosition = targetPosition;
        }
    }
}