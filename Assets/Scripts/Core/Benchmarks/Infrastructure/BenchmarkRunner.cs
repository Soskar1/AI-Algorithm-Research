using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Benchmarks.Domain;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Matches.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using Reflex.Attributes;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace AiAlgorithmsResearch.Core.Benchmarks.Infrastructure
{
    internal class BenchmarkRunner : MonoBehaviour
    {
        [SerializeField] private List<BenchmarkConfigurationAsset> _benchmarkConfigurationAssets;
        [SerializeField] private CombatAgentType _firstTeamAgent;
        [SerializeField] private CombatAgentType _secondTeamAgent;
        private int _matchCount;
        private int _currentMatch = 0;
        private int _currentConfig = 0;

        private IMatchRunner _matchRunner;
        private IEntityFactory _entityFactory;
        private ICombatAgentFactory _combatAgentFactory;
        private IWorldGenerator _worldGenerator;
        private IMapEditor _mapEditor;
        private BenchmarkConfiguration _configuration;
        private MatchInitializationRequest _matchInitializationRequest;

        private IMatchView _matchView;
        private List<int> _currentMatchWinnerCount = new();
        private List<int> _overallMatchWinnerCounts = new();

        [Header("UI")]
        [SerializeField] private GameObject _ui;
        [SerializeField] private TextMeshProUGUI _matchCountText;
        [SerializeField] private TextMeshProUGUI _firstAlgorithmText;
        [SerializeField] private TextMeshProUGUI _secondAlgorithmText;
        [SerializeField] private Image _progressBar;

        [SerializeField] private List<int> _seeds;
        [SerializeField] private List<int> _seeds2;

        [SerializeField] private GameObject _details;
        [SerializeField] private TextMeshProUGUI _detailedTextPrefab;

        [SerializeField] private TextMeshProUGUI _seedText;

        private Random _random;

        [Inject]
        public void Inject(IMatchRunner matchRunner, IEntityFactory entityFactory, ICombatAgentFactory agentFactory, IWorldGenerator worldGenerator, IMapEditor mapEditor)
        {
            _matchRunner = matchRunner;
            _entityFactory = entityFactory;
            _combatAgentFactory = agentFactory;
            _worldGenerator = worldGenerator;
            _mapEditor = mapEditor;
        }

        public void Start()
        {
            _overallMatchWinnerCounts.Add(0);
            _overallMatchWinnerCounts.Add(0);
            _currentMatchWinnerCount.Add(0);
            _currentMatchWinnerCount.Add(0);

            _matchCount = _seeds.Count * _benchmarkConfigurationAssets.Count;
            StartNewMatch();
        }

        public void Update()
        {
            if (_matchView == null)
                return;

            if (_matchView.State == MatchState.Finished || _matchView.State == MatchState.NotStarted)
                return;

            _matchRunner.Tick();

            if (_matchView.State == MatchState.Finished)
            {
                ++_overallMatchWinnerCounts[_matchView.Winner.Value];
                ++_currentMatchWinnerCount[_matchView.Winner.Value];
                ++_currentMatch;

                DisplayStatistics();

                if (_currentMatch < _seeds.Count)
                {
                    StartNewMatch();
                }
                else
                {
                    AppendDataToDetails();

                    if (_currentConfig < _benchmarkConfigurationAssets.Count - 1)
                    {
                        ++_currentConfig;
                        _currentMatch = 0;

                        _currentMatchWinnerCount.Clear();
                        _currentMatchWinnerCount.Add(0);
                        _currentMatchWinnerCount.Add(0);

                        StartNewMatch();
                    }
                }
            }
        }

        private void StartNewMatch()
        {
            _random = new Random(_seeds[_currentMatch]);
            _seedText.text = _seeds[_currentMatch].ToString();
            var configAsset = _benchmarkConfigurationAssets[_currentConfig];
            _configuration = configAsset.ToConfiguration(_firstTeamAgent.ToString(), _secondTeamAgent.ToString());

            _mapEditor.Clear();
            _worldGenerator.Generate(_configuration.WorldWidth, _configuration.WorldHeight, _configuration.Walls);

            List<BattleParticipantSetup> battleParticipants = new();
            var teams = _configuration.Teams.Keys.ToList();

            foreach (var team in teams)
            {
                var teamEntities = _configuration.Teams[team];

                foreach ((var position, var entityDefinitionId) in teamEntities)
                {
                    var entityDefinition = _configuration.EntityDefinitionsById[entityDefinitionId];
                    var entity = _entityFactory.CreateEntity(entityDefinition);
                    var actionDefinitions = _configuration.ActionsByEntity[entityDefinitionId];
                    var battleParticipantSetup = new BattleParticipantSetup(entity, position, team, actionDefinitions);

                    battleParticipants.Add(battleParticipantSetup);
                }
            }

            var agentsByTeam = new Dictionary<TeamId, ICombatAgent>();
            var firstTeamAgent = CreateAgent(_firstTeamAgent);
            var secondTeamAgent = CreateAgent(_secondTeamAgent);

            agentsByTeam.Add(teams[0], firstTeamAgent);
            agentsByTeam.Add(teams[1], secondTeamAgent);
            
            var battleRequest = new BattleInitializationRequest(battleParticipants);
            _matchInitializationRequest = new MatchInitializationRequest(battleRequest, agentsByTeam);

            _matchView = _matchRunner.StartMatch(_matchInitializationRequest, _random);
            if (_matchView == null)
            {
                Debug.LogError("Match is not started!");
            }
        }

        private void DisplayStatistics()
        {
            _matchCountText.text = $"Scenario {_benchmarkConfigurationAssets[_currentConfig].name}: {_seeds.Count} matches";

            var teams = _configuration.Teams.Keys.ToList();
            var firstTeam = teams[0];
            var secondTeam = teams[1];

            var firstTeamWon = _overallMatchWinnerCounts[firstTeam.Value];
            var secondTeamWon = _overallMatchWinnerCounts[secondTeam.Value];

            var playedMatches = _currentConfig * _seeds.Count + _currentMatch;

            _firstAlgorithmText.text = $"{firstTeam.DisplayName} won: {firstTeamWon} ({(firstTeamWon / (float)playedMatches) * 100:F2}% win rate)";
            _secondAlgorithmText.text = $"{secondTeam.DisplayName} won: {secondTeamWon} ({(secondTeamWon / (float)playedMatches) * 100:F2}% win rate)";

            _progressBar.fillAmount = playedMatches / (float)_matchCount;
        }

        private void AppendDataToDetails()
        {
            var teams = _configuration.Teams.Keys.ToList();
            var firstTeam = teams[0];
            var secondTeam = teams[1];

            var firstTeamWon = _currentMatchWinnerCount[firstTeam.Value];
            var secondTeamWon = _currentMatchWinnerCount[secondTeam.Value];

            var textInstance = Instantiate(_detailedTextPrefab, _details.transform);
            var scenarioName = _benchmarkConfigurationAssets[_currentConfig].name;

            if (firstTeamWon > secondTeamWon)
            {
                var winRate = (firstTeamWon / (float)_seeds.Count) * 100;
                textInstance.text = $"{scenarioName}: {firstTeam.DisplayName} ({winRate:F2}%)";
            }
            else if (firstTeamWon < secondTeamWon)
            {
                var winRate = (secondTeamWon / (float)_seeds.Count) * 100;
                textInstance.text = $"{scenarioName}: {secondTeam.DisplayName} ({winRate:F2}%)";
            }
            else
            {
                textInstance.text = $"{scenarioName}: draw";
            }
        }

        private ICombatAgent CreateAgent(CombatAgentType agentType)
        {
            switch (agentType)
            {
                case CombatAgentType.Random:
                    return _combatAgentFactory.CreateRandomAgent(_random);

                case CombatAgentType.StateMachine:
                    return _combatAgentFactory.CreateStateMachineAgent();

                case CombatAgentType.Minimax:
                    return _combatAgentFactory.CreateMinimaxAgent(4);
            }

            return null;
        }
    }
}
