using UnityEngine;

namespace AiAlgorithmsResearch.Core.Maps.Api
{
    public interface IReadOnlyTileMap
    {
        bool TryGetNode(Vector2Int position, out MapNodeReadOnly readonlyNode);
    }
}
