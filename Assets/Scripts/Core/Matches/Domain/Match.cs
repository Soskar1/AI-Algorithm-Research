using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Matches.Api;

namespace AiAlgorithmsResearch.Core.Matches.Domain
{
    internal sealed class Match : IMatchView
    {
        public MatchState State { get; private set; }
        public TeamId Winner { get; private set; }
        public IBattle Battle { get; private set; }
        public IBattleParticipant CurrentParticipant => Battle.Current;
        public int PlayedTurns { get; private set; }

        public Match()
        {
            State = MatchState.NotStarted;
            PlayedTurns = 0;
        }

        public void Start(IBattle battle)
        {
            Battle = battle;
            State = MatchState.Running;
            Winner = new TeamId(-1);
        }

        public void Finish(TeamId teamId)
        {
            State = MatchState.Finished;
            Winner = teamId;
        }

        public void NextTurn()
        {
            Battle.NextTurn();
            ++PlayedTurns;
        }
    }
}