using UnityEngine;

namespace AiAlgorithmsResearch.Core.Maps.Api
{
    public interface IMapEditor
    {
        bool AddTile(Vector2Int position, MapNodeType type);
        bool SetTileType(Vector2Int position, MapNodeType type);
        bool Connect(Vector2Int a, Vector2Int b);
    }
}
