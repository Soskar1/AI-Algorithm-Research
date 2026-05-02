using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Matches.Api;
using AiAlgorithmsResearch.Core.Matches.Domain;
using System.Collections.Generic;
using System.Linq;

namespace AiAlgorithmsResearch.Core.Matches.Application
{
    internal sealed class MatchRunner : IMatchRunner
    {
        private readonly IBattleInitializer _battleInitializer;
        private readonly IActionCooldownEditor _cooldownEditor;
        private readonly IEntityEnergyEditor _energyEditor;
        private readonly IStunStatusEditor _stunStatusEditor;
        private readonly ICombatActionProvider _actionProvider;
        private readonly ICombatActionExecutor _actionExecutor;
        private readonly TeamId _teamA;
        private readonly TeamId _teamB;

        private IReadOnlyDictionary<TeamId, ICombatAgent> _agentsByTeam;

        private Match _match;

        public MatchRunner(
            TeamId teamA,
            TeamId teamB,
            IBattleInitializer battleInitializer,
            IActionCooldownEditor cooldownEditor,
            IEntityEnergyEditor energyEditor,
            IStunStatusEditor stunEditor,
            ICombatActionExecutor combatActionExecutor
            )
        {
            _teamA = teamA;
            _teamB = teamB;
            _battleInitializer = battleInitializer;
            _cooldownEditor = cooldownEditor;
            _energyEditor = energyEditor;
            _stunStatusEditor = stunEditor;
            _actionExecutor = combatActionExecutor;
        }

        public IMatchView StartMatch(MatchInitializationRequest request)
        {
            var battle = _battleInitializer.StartBattle(request.BattleRequest);

            if (battle == null)
            {
                return null;
            }

            _agentsByTeam = request.AgentsByTeam;

            _match = new Match();
            _match.Start(battle);

            return _match;
        }

        public void Tick()
        {
            if (_match == null || _match.State != MatchState.Running)
            {
                return;
            }

            var current = _match.CurrentParticipant;

            _cooldownEditor.TickCooldowns(current.Entity);
            _energyEditor.RegenerateEnergy(current.Entity);

            if (_stunStatusEditor.ConsumeStun(current.Entity))
            {
                _match.NextTurn();
                return;
            }

            if (!_agentsByTeam.TryGetValue(current.TeamId, out var agent))
            {
                return;
            }

            var actions = _actionProvider.GetAvailableActions(_match.Battle, current);

            var combatAgentContext = new CombatAgentContext(current.Entity, actions);
            var action = agent.ChooseAction(combatAgentContext);

            _actionExecutor.TryExecute(action);

            CheckWinCondition();

            if (_match.State == MatchState.Running)
            {
                _match.NextTurn();
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