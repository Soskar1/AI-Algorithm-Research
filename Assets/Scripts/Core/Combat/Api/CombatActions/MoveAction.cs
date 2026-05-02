using AiAlgorithmsResearch.Core.Entities.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct MoveAction : ICombatAction
    {
        public IEntityView Actor { get; }
        public CombatActionId Id => CombatActionIds.Move;
        public Vector2Int TargetPosition { get; }

        public MoveAction(IEntityView actor, Vector2Int targetPosition)
        {
            Actor = actor;
            TargetPosition = targetPosition;
        }
    }
}