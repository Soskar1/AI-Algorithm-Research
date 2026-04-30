using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Worlds.Domain
{
    internal class World : IWorldView
    {
        public IMapEditor MapEditor { get; }
        public IReadOnlyTileMap Map { get; }

        public World(IMapEditor mapEditor, IReadOnlyTileMap readOnlyTileMap)
        {
            MapEditor = mapEditor;
            Map = readOnlyTileMap;
        }

        public void AddTile(Vector2Int position, MapNodeType mapNodeType)
        {
            MapEditor.AddTile(position, mapNodeType);
        }

        public void Connect(Vector2Int a, Vector2Int b)
        {
            MapEditor.Connect(a, b);
        }
    }
}