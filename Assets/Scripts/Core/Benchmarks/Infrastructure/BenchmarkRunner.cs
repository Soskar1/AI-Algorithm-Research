using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Benchmarks.Domain;
using AiAlgorithmsResearch.Core.Benchmarks.Presentation;
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
        private class MatchEntry
        {
            public BenchmarkConfiguration Configuration { get; }
            public int Seed { get; }
            public Dictionary<CombatAgentType, IReadOnlyDictionary<Vector2Int, EntityDefinitionId>> AgentTeams { get; }

            public MatchEntry(BenchmarkConfiguration configuration, int seed, CombatAgentType firstTeamAgent, CombatAgentType secondTeamAgent)
            {
                Configuration = configuration;
                Seed = seed;

                AgentTeams = new()
                {
                    { firstTeamAgent, Configuration.Teams[0] },
                    { secondTeamAgent, Configuration.Teams[1] }
                };
            }
        }

        [SerializeField] private List<BenchmarkConfigurationAsset> _benchmarkConfigurationAssets;
        [SerializeField] private CombatAgentType _firstAgent;
        [SerializeField] private CombatAgentType _secondAgent;

        private List<MatchEntry> _matchesToPlay;

        private int _matchCount;
        private int _currentMatch = 0;
        private int _currentConfig = 0;
        private int _configSwitch;

        private IMatchRunner _matchRunner;
        private IEntityFactory _entityFactory;
        private ICombatAgentFactory _combatAgentFactory;
        private IWorldGenerator _worldGenerator;
        private IMapEditor _mapEditor;
        private MatchInitializationRequest _matchInitializationRequest;

        private IMatchView _matchView;

        private Dictionary<TeamId, CombatAgentType> _agentTeams = new();
        private Dictionary<CombatAgentType, int> _currentConfigurationWinnerCount = new();
        private Dictionary<CombatAgentType, int> _overallBenchmarkWinnerCount = new();

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
        [SerializeField] private ActionExecutionStatistics _actionExecutionStatistics;

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
            _overallBenchmarkWinnerCount.Add(_firstAgent, 0);
            _overallBenchmarkWinnerCount.Add(_secondAgent, 0);
            _currentConfigurationWinnerCount.Add(_firstAgent, 0);
            _currentConfigurationWinnerCount.Add(_secondAgent, 0);

            _actionExecutionStatistics.Initialize(_firstAgent, _secondAgent);
            _matchCountText.text = $"Scenario {_benchmarkConfigurationAssets[_currentConfig].name}";

            PredefineMathes();
            StartNewMatch();
        }

        private void PredefineMathes()
        {
            _matchesToPlay = new();

            foreach (var asset in _benchmarkConfigurationAssets)
            {
                var configuration = asset.ToConfiguration();

                foreach (var seed in _seeds)
                {
                    var first = new MatchEntry(configuration, seed, _firstAgent, _secondAgent);
                    var second = new MatchEntry(configuration, seed, _secondAgent, _firstAgent);
                    _matchesToPlay.Add(first);
                    _matchesToPlay.Add(second);
                }
            }

            _configSwitch = _seeds.Count * 2;
            _matchCount = _matchesToPlay.Count;
        }

        public void Update()
        {
            if (_matchView == null)
                return;

            if (_matchView.State == MatchState.Finished || _matchView.State == MatchState.NotStarted)
                return;

            _matchRunner.Tick();

            var lastTeam = _matchView.LastTeam;
            var lastExecutedActions = _matchView.ExecutedActions;

            if (lastExecutedActions != null)
            {
                foreach (var action in lastExecutedActions)
                {
                    var agent = _agentTeams[lastTeam];
                    _actionExecutionStatistics.DisplayExecutedAction(agent, action);
                }
            }

            if (_matchView.State == MatchState.Finished)
            {
                var winnerAgent = _agentTeams[_matchView.Winner];

                ++_overallBenchmarkWinnerCount[winnerAgent];
                ++_currentConfigurationWinnerCount[winnerAgent];
                ++_currentMatch;

                DisplayStatistics();

                if (_currentMatch < _matchesToPlay.Count)
                {
                    StartNewMatch();

                    if (_currentMatch % _configSwitch == 0)
                    {
                        ++_currentConfig;
                        _matchCountText.text = $"Scenario {_benchmarkConfigurationAssets[_currentConfig].name}";

                        AppendDataToDetails();

                        _currentConfigurationWinnerCount.Clear();
                        _currentConfigurationWinnerCount.Add(_firstAgent, 0);
                        _currentConfigurationWinnerCount.Add(_secondAgent, 0);
                    }
                }
            }
        }

        private void StartNewMatch()
        {
            var matchToPlay = _matchesToPlay[_currentMatch];
            var configuration = matchToPlay.Configuration;
            _seedText.text = matchToPlay.Seed.ToString();

            _agentTeams.Clear();
            _mapEditor.Clear();
            _worldGenerator.Generate(configuration.WorldWidth, configuration.WorldHeight, configuration.Walls);

            List<BattleParticipantSetup> battleParticipants = new();
            var teamConfiguration = configuration.Teams;
            var currentIndex = 0;

            foreach ((var agent, var entities) in matchToPlay.AgentTeams)
            {
                var teamId = new TeamId(currentIndex);
                _agentTeams.Add(teamId, agent);
                ++currentIndex;

                foreach ((var position, var entityDefinitionId) in entities)
                {
                    var entityDefinition = configuration.EntityDefinitionsById[entityDefinitionId];
                    var entity = _entityFactory.CreateEntity(entityDefinition);
                    var actionDefinitions = configuration.ActionsByEntity[entityDefinitionId];
                    var battleParticipantSetup = new BattleParticipantSetup(entity, position, teamId, actionDefinitions);

                    battleParticipants.Add(battleParticipantSetup);
                }
            }

            var random = new Random(matchToPlay.Seed);
            var firstTeamAgent = CreateAgent(matchToPlay.AgentTeams.Keys.First(), random);
            var secondTeamAgent = CreateAgent(matchToPlay.AgentTeams.Keys.Last(), random);

            var teams = _agentTeams.Keys.ToList();

            var agentsByTeam = new Dictionary<TeamId, ICombatAgent>
            {
                { teams[0], firstTeamAgent },
                { teams[1], secondTeamAgent }
            };

            var battleRequest = new BattleInitializationRequest(battleParticipants);
            _matchInitializationRequest = new MatchInitializationRequest(battleRequest, agentsByTeam);

            _matchView = _matchRunner.StartMatch(_matchInitializationRequest, random);
            if (_matchView == null)
            {
                Debug.LogError("Match is not started!");
            }
        }

        private void DisplayStatistics()
        {
            var firstAgentWon = _overallBenchmarkWinnerCount[_firstAgent];
            var secondAgentWon = _overallBenchmarkWinnerCount[_secondAgent];

            _firstAlgorithmText.text = $"{_firstAgent} won: {firstAgentWon} ({(firstAgentWon / (float)_currentMatch) * 100:F2}% win rate)";
            _secondAlgorithmText.text = $"{_secondAgent} won: {secondAgentWon} ({(secondAgentWon / (float)_currentMatch) * 100:F2}% win rate)";

            _progressBar.fillAmount = _currentMatch / (float)_matchCount;
        }

        private void AppendDataToDetails()
        {
            var firstTeamWon = _currentConfigurationWinnerCount[_firstAgent];
            var secondTeamWon = _currentConfigurationWinnerCount[_secondAgent];

            var textInstance = Instantiate(_detailedTextPrefab, _details.transform);
            var scenarioName = _benchmarkConfigurationAssets[_currentConfig].name;

            if (firstTeamWon > secondTeamWon)
            {
                var winRate = (firstTeamWon / (float)_seeds.Count) * 100;
                textInstance.text = $"{scenarioName}: {_firstAgent} ({winRate:F2}%)";
            }
            else if (firstTeamWon < secondTeamWon)
            {
                var winRate = (secondTeamWon / (float)_seeds.Count) * 100;
                textInstance.text = $"{scenarioName}: {_secondAgent} ({winRate:F2}%)";
            }
            else
            {
                textInstance.text = $"{scenarioName}: draw";
            }
        }

        private ICombatAgent CreateAgent(CombatAgentType agentType, Random random)
        {
            switch (agentType)
            {
                case CombatAgentType.Random:
                    return _combatAgentFactory.CreateRandomAgent(random);

                case CombatAgentType.StateMachine:
                    return _combatAgentFactory.CreateStateMachineAgent();

                case CombatAgentType.Minimax1:
                    return _combatAgentFactory.CreateMinimaxAgent(1);

                case CombatAgentType.Minimax4:
                    return _combatAgentFactory.CreateMinimaxAgent(4);
            }

            return null;
        }
    }
}
