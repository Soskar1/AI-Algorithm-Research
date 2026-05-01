using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Maps.Api;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Worlds.Api
{
    public interface IWorldView
    {
        IReadOnlyTileMap Map { get; }
        IReadOnlyCollection<WorldEntityView> Entities { get; }
        bool TryGetEntityPosition(IEntityView entity, out Vector2Int position);
    }
}
