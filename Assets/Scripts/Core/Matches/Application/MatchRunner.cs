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
        private readonly IWorldEditor _worldEditor;
        private readonly ICombatLogger _combatLogger;
        private readonly IRuntimeCombatStateFactory _runtimeCombatStateFactory;

        private Match _match;
        private TeamId _teamA;
        private TeamId _teamB;
        private IReadOnlyDictionary<TeamId, ICombatAgent> _agentsByTeam;

        private ICombatStateView _currentStateView;
        private ICombatStateEditor _currentStateEditor;

        private int _currentTurn = 0;
        private bool _log = false;

        public MatchRunner(
            IBattleInitializer battleInitializer,
            IActionCooldownEditor cooldownEditor,
            IEntityEnergyEditor energyEditor,
            IStunStatusEditor stunEditor,
            ICombatActionExecutor combatActionExecutor,
            IWorldEditor worldEditor,
            ICombatLogger combatLogger,
            IRuntimeCombatStateFactory runtimeCombatStateFactory,
            bool log = false)
        {
            _battleInitializer = battleInitializer;
            _cooldownEditor = cooldownEditor;
            _energyEditor = energyEditor;
            _stunStatusEditor = stunEditor;
            _actionExecutor = combatActionExecutor;
            _worldEditor = worldEditor;
            _combatLogger = combatLogger;
            _runtimeCombatStateFactory = runtimeCombatStateFactory;
            _log = log;
        }

        public IMatchView StartMatch(MatchInitializationRequest request, Random random)
        {
            _cooldownEditor.Clear();
            _stunStatusEditor.Clear();
            _worldEditor.Clear();
            _currentTurn = 0;

            var battle = _battleInitializer.StartBattle(request.BattleRequest, random);

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

            (_currentStateView, _currentStateEditor) = _runtimeCombatStateFactory.Create(battle);

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

            var entityLog = _combatLogger.GetEntityRepresentation(current.Entity.Id, _currentStateView);
            if (_log)
            {
                Debug.Log($"[{_currentTurn}] {entityLog} started it's turn");
            }

            _cooldownEditor.TickCooldowns(current.Entity.Id);
            _energyEditor.RegenerateEnergy(current.Entity);

            if (_stunStatusEditor.ConsumeStun(current.Entity.Id))
            {
                _combatLogger.Log(current.Entity.Id, "skipping it's turn due to stun status", _currentStateView);
                _match.NextTurn();
                ++_currentTurn;
                return;
            }

            if (!_agentsByTeam.TryGetValue(current.TeamId, out var agent))
            {
                if (_log)
                {
                    Debug.LogError($"Agent for {current.TeamId.Value} team is not found!");
                }
                ++_currentTurn;
                return;
            }

            var actions = new List<ICombatActionDefinition>();
            foreach (var actionDefinition in current.ActionDefinitions)
            {
                actions.Add(actionDefinition);
            }

            var plan = agent.ChoosePlan(_currentStateView, current.Entity.Id);

            foreach (var action in plan.Actions)
            {
                var executionResult = _actionExecutor.TryExecute(action, _currentStateView, _currentStateEditor);

                if (!executionResult)
                {
                    Debug.LogError($"[{_currentTurn}] {entityLog} failed to execute {action.Id.Value} action...");
                }
                else
                {
                    if (_log)
                    {
                        entityLog = _combatLogger.GetEntityRepresentation(current.Entity.Id, _currentStateView);
                        Debug.Log($"[{_currentTurn}] {entityLog} executed it's action.");
                    }
                }

                var matchEnded = CheckWinCondition();
                if (matchEnded)
                {
                    break;
                }
            }

            if (_match.State == MatchState.Running)
            {
                if (_log)
                {
                    Debug.Log("No winner!");
                }

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

            _match.Finish(teamAAlive ? _teamA : _teamB);
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