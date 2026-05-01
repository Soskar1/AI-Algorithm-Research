using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Worlds.Domain
{
    internal class World : IWorldView
    {
        private readonly List<WorldEntity> _entities = new();
        public IMapEditor MapEditor { get; }
        public IReadOnlyTileMap Map { get; }

        public IReadOnlyCollection<WorldEntityView> Entities => _entities
            .Select(worldEntity => new WorldEntityView(worldEntity.Entity, worldEntity.Position))
            .ToArray();

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

        public void AddEntity(IEntityView entity, Vector2Int position)
        {
            var worldEntity = new WorldEntity(entity, position);
            _entities.Add(worldEntity);
        }

        internal bool TryMoveEntity(IEntityView entity, Vector2Int newPosition)
        {
            var worldEntity = _entities.FirstOrDefault(e => ReferenceEquals(e.Entity, entity));
            
            if (worldEntity == null)
            {
                return false;
            }

            worldEntity.MoveTo(newPosition);
            return true;
        }

        internal bool IsOccupied(Vector2Int position)
        {
            return _entities.Any(entity => entity.Position == position);
        }

        public bool TryGetNode(Vector2Int position, out MapNodeReadOnly readonlyNode)
        {
            return Map.TryGetNode(position, out readonlyNode);
        }
    }
}