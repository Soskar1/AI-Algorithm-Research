using AiAlgorithmsResearch.Core.Entities.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Worlds.Domain
{
    internal class WorldEntity
    {
        public IEntityView Entity { get; }
        public Vector2Int Position { get; private set; }

        public WorldEntity(IEntityView entity, Vector2Int position)
        {
            Entity = entity;
            Position = position;
        }

        public void MoveTo(Vector2Int position)
        {
            Position = position;
        }
    }
}
