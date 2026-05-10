using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Matches.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Matches.Domain
{
    internal sealed class Match : IMatchView
    {
        public MatchState State { get; private set; }
        public TeamId Winner { get; private set; }
        public IBattle Battle { get; private set; }
        public IBattleParticipant CurrentParticipant => Battle.Current;
        public int PlayedTurns { get; private set; }

        public TeamId LastTeam { get; private set; }
        public List<CombatActionId> ExecutedActions { get; private set; }

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

        public void NextTurn(List<CombatActionId> executedActions)
        {
            LastTeam = Battle.Current.TeamId;
            ExecutedActions = executedActions;

            Battle.NextTurn();
            ++PlayedTurns;
        }
    }
}