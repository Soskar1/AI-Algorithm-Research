using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Maps.Domain;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Maps.Application
{
    internal class MapEditor : IMapEditor
    {
        private readonly Map _map;

        public MapEditor(Map map)
        {
            _map = map;
        }

        public bool AddTile(Vector2Int position, MapNodeType type)
        {
            return _map.TryAddNode(position, type);
        }

        public bool Connect(Vector2Int a, Vector2Int b)
        {
            return _map.ConnectNodes(a, b);
        }

        public bool SetTileType(Vector2Int position, MapNodeType type)
        {
            return _map.ChangeNodeType(position, type);
        }
    }
}
