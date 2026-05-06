using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Matches.Api;
using AiAlgorithmsResearch.Core.Matches.Domain;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace AiAlgorithmsResearch.Core.Matches.Application
{
    internal sealed class MatchRunner : IMatchRunner
    {
        private readonly IBattleInitializer _battleInitializer;
        private readonly IActionCooldownEditor _cooldownEditor;
        private readonly IEntityEnergyEditor _energyEditor;
        private readonly IStunStatusEditor _stunStatusEditor;
        private readonly ICombatActionExecutor _actionExecutor;
        private readonly IWorldView _worldView;
        private readonly IAiEngine _aiEngine;
        private readonly ICombatLogger _combatLogger;

        private Match _match;
        private TeamId _teamA;
        private TeamId _teamB;
        private IReadOnlyDictionary<TeamId, ICombatAgent> _agentsByTeam;

        private int _currentTurn = 0;

        public MatchRunner(
            IBattleInitializer battleInitializer,
            IActionCooldownEditor cooldownEditor,
            IEntityEnergyEditor energyEditor,
            IStunStatusEditor stunEditor,
            ICombatActionExecutor combatActionExecutor,
            IWorldView worldView,
            IAiEngine aiEngine,
            ICombatLogger combatLogger
            )
        {
            _battleInitializer = battleInitializer;
            _cooldownEditor = cooldownEditor;
            _energyEditor = energyEditor;
            _stunStatusEditor = stunEditor;
            _actionExecutor = combatActionExecutor;
            _worldView = worldView;
            _aiEngine = aiEngine;
            _combatLogger = combatLogger;
        }

        public IMatchView StartMatch(MatchInitializationRequest request)
        {
            var battle = _battleInitializer.StartBattle(request.BattleRequest);

            if (battle == null)
            {
                return null;
            }

            _agentsByTeam = request.AgentsByTeam;
            var teams = _agentsByTeam.Keys.ToList();
            if (teams.Count != 2)
            {
                throw new Exception("Only two teams allowed");
            }
            _teamA = teams[0];
            _teamB = teams[1];

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

            var entityLog = _combatLogger.GetEntityRepresentation(current.Entity);
            Debug.Log($"[{_currentTurn}] {entityLog} started it's turn");

            _cooldownEditor.TickCooldowns(current.Entity);
            _energyEditor.RegenerateEnergy(current.Entity);

            if (_stunStatusEditor.ConsumeStun(current.Entity))
            {
                _combatLogger.Log(current.Entity, "skipping it's turn due to stun status");
                _match.NextTurn();
                ++_currentTurn;
                return;
            }

            if (!_agentsByTeam.TryGetValue(current.TeamId, out var agent))
            {
                Debug.LogError($"Agent for {current.TeamId.Value} team is not found!");
                ++_currentTurn;
                return;
            }

            var actions = new List<ICombatActionDefinition>();
            foreach (var actionDefinition in current.ActionDefinitions)
            {
                actions.Add(actionDefinition);
            }

            var combatAgentContext = new CombatAgentContext(current, _worldView, _match.Battle);
            var plan = _aiEngine.ProduceMove(agent, combatAgentContext);

            foreach (var action in plan.Actions)
            {
                var executionResult = _actionExecutor.TryExecute(action);

                if (!executionResult)
                {
                    Debug.LogError($"[{_currentTurn}] {entityLog} failed to execute {action.Id.Value} action...");
                }
                else
                {
                    entityLog = _combatLogger.GetEntityRepresentation(current.Entity);
                    Debug.Log($"[{_currentTurn}] {entityLog} executed it's action.");
                }

                var matchEnded = CheckWinCondition();
                if (matchEnded)
                {
                    break;
                }
            }

            if (_match.State == MatchState.Running)
            {
                Debug.Log("No winner!");
                ++_currentTurn;
                _match.NextTurn();
            }
        }

        private bool CheckWinCondition()
        {
            var teamAAlive = IsTeamAlive(_teamA);
            var teamBAlive = IsTeamAlive(_teamB);

            if (teamAAlive && teamBAlive)
                return false;

            _match.Finish(teamAAlive ? MatchWinner.TeamA : MatchWinner.TeamB);
            return true;
        }

        private bool IsTeamAlive(TeamId teamId)
        {
            return _match.Battle.TurnOrder.Any(participant =>
                participant.TeamId.Value == teamId.Value &&
                participant.Entity.Health.Current > 0);
        }
    }
}