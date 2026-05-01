using AiAlgorithmsResearch.Core.Entities.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Worlds.Api
{
    public struct WorldEntityView
    {
        public IEntityView Entity { get; }
        public Vector2Int Position { get; }

        public WorldEntityView(IEntityView entity, Vector2Int position)
        {
            Entity = entity;
            Position = position;
        }
    }
}
