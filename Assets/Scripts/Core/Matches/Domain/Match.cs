using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Matches.Api;

namespace AiAlgorithmsResearch.Core.Matches.Domain
{
    internal sealed class Match : IMatchView
    {
        public MatchState State { get; private set; }
        public MatchWinner Winner { get; private set; }
        public IBattle Battle { get; private set; }

        public Match()
        {
            State = MatchState.NotStarted;
            Winner = MatchWinner.None;
        }

        public void Start(IBattle battle)
        {
            Battle = battle;
            State = MatchState.Running;
            Winner = MatchWinner.None;
        }

        public void Finish(MatchWinner winner)
        {
            State = MatchState.Finished;
            Winner = winner;
        }
    }
}