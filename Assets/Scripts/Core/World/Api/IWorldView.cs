using AiAlgorithmsResearch.Core.Maps.Api;

namespace AiAlgorithmsResearch.Core.Worlds.Api
{
    public interface IWorldView
    {
        IReadOnlyTileMap Map { get; }
    }
}
