using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using AiAlgorithmsResearch.Core.Worlds.Domain;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Worlds.Application
{
    internal class WorldEditor : IWorldEditor
    {
        private readonly World _world;

        public WorldEditor(World world)
        {
            _world = world;
        }

        public bool TryAddEntity(IEntityView entity, Vector2Int position)
        {
            if (!_world.TryGetNode(position, out var node))
                return false;

            if (node.Type == MapNodeType.Obstacle)
                return false;

            if (_world.IsOccupied(position))
                return false;

            _world.AddEntity(entity, position);
            return true;
        }

        public bool TryMoveEntity(IEntityView entity, Vector2Int newPosition)
        {
            if (!_world.TryGetNode(newPosition, out var node))
                return false;

            if (node.Type == MapNodeType.Obstacle)
                return false;

            if (_world.IsOccupied(newPosition))
                return false;

            return _world.TryMoveEntity(entity, newPosition);
        }
    }
}
