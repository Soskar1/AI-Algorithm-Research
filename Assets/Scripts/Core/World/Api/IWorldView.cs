using AiAlgorithmsResearch.Core.Maps.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Worlds.Api
{
    public interface IWorldView
    {
        IReadOnlyTileMap Map { get; }
        IReadOnlyCollection<WorldEntityView> Entities { get; }
    }
}
