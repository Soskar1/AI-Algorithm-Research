using AiAlgorithmsResearch.Core.Entities.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Worlds.Api
{
    public interface IWorldEditor
    {
        bool TryAddEntity(IEntityView entity, Vector2Int position);
        bool TryMoveEntity(IEntityView entity, Vector2Int newPosition);
    }
}
