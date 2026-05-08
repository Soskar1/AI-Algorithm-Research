using AiAlgorithmsResearch.Core.Combat.Api;

namespace AiAlgorithmsResearch.Core.Matches.Api
{
    public interface IMatchView
    {
        MatchState State { get; }
        TeamId Winner { get; }
        IBattle Battle { get; }
    }
}