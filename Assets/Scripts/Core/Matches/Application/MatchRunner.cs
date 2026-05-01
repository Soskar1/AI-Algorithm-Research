using System.Linq;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Matches.Api;
using AiAlgorithmsResearch.Core.Matches.Domain;

namespace AiAlgorithmsResearch.Core.Matches.Application
{
    internal sealed class MatchRunner : IMatchRunner
    {
        private readonly IBattleInitializer _battleInitializer;
        private readonly TeamId _teamA;
        private readonly TeamId _teamB;

        private Match _match;

        public MatchRunner(IBattleInitializer battleInitializer, TeamId teamA, TeamId teamB)
        {
            _battleInitializer = battleInitializer;
            _teamA = teamA;
            _teamB = teamB;
        }

        public IMatchView StartMatch(BattleInitializationRequest battleRequest)
        {
            var battle = _battleInitializer.StartBattle(battleRequest);

            if (battle == null)
            {
                return null;
            }

            _match = new Match();
            _match.Start(battle);

            return _match;
        }

        public void Tick()
        {
            if (_match == null || _match.State != MatchState.Running)
                return;

            // TODO:
            // 1. get current participant
            // 2. ask AI agent for action
            // 3. execute action

            CheckWinCondition();

            if (_match.State == MatchState.Running)
            {
                _match.Battle.NextTurn();
            }
        }

        private void CheckWinCondition()
        {
            var teamAAlive = IsTeamAlive(_teamA);
            var teamBAlive = IsTeamAlive(_teamB);

            if (teamAAlive && teamBAlive)
                return;

            _match.Finish(teamAAlive ? MatchWinner.TeamA : MatchWinner.TeamB);
        }

        private bool IsTeamAlive(TeamId teamId)
        {
            return _match.Battle.TurnOrder.Any(participant =>
                participant.TeamId.Value == teamId.Value &&
                participant.Entity.Health.Current > 0);
        }
    }
}