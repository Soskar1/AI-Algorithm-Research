using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Matches.Api
{
    public interface IMatchView
    {
        MatchState State { get; }
        TeamId Winner { get; }
        IBattle Battle { get; }
        TeamId LastTeam { get; }
        List<CombatActionId> ExecutedActions { get; }
    }
}