using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Benchmarks.Domain;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
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
        [SerializeField] private BenchmarkConfigurationAsset _benchmarkConfigurationAsset;
        private int _matchCount;
        private int _currentMatch = 0;

        private IMatchRunner _matchRunner;
        private IEntityFactory _entityFactory;
        private ICombatAgentFactory _combatAgentFactory;
        private IWorldGenerator _worldGenerator;
        private BenchmarkConfiguration _configuration;
        private MatchInitializationRequest _matchInitializationRequest;

        private IMatchView _matchView;
        private Dictionary<int, int> _matchWinnerCount = new();

        [Header("UI")]
        [SerializeField] private GameObject _ui;
        [SerializeField] private TextMeshProUGUI _matchCountText;
        [SerializeField] private TextMeshProUGUI _firstAlgorithmText;
        [SerializeField] private TextMeshProUGUI _secondAlgorithmText;
        [SerializeField] private Image _progressBar;

        [SerializeField] private List<int> _seeds;

        private Random _random;

        [Inject]
        public void Inject(IMatchRunner matchRunner, IEntityFactory entityFactory, ICombatAgentFactory agentFactory, IWorldGenerator worldGenerator)
        {
            _matchRunner = matchRunner;
            _entityFactory = entityFactory;
            _combatAgentFactory = agentFactory;
            _worldGenerator = worldGenerator;
        }

        public void Start()
        {
            _matchWinnerCount.Add(_benchmarkConfigurationAsset.Teams[0].TeamId, 0);
            _matchWinnerCount.Add(_benchmarkConfigurationAsset.Teams[1].TeamId, 0);
            _matchCount = _seeds.Count;
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
                ++_matchWinnerCount[_matchView.Winner.Value];
                ++_currentMatch;

                if (_currentMatch < _matchCount)
                {
                    StartNewMatch();
                }

                DisplayStatistics();
            }
        }

        private void StartNewMatch()
        {
            _random = new Random(_seeds[_currentMatch]);
            _configuration = _benchmarkConfigurationAsset.ToConfiguration(_combatAgentFactory, _random);

            _worldGenerator.Generate(_configuration.WorldWidth, _configuration.WorldHeight);

            List<BattleParticipantSetup> battleParticipants = new();
            foreach (var team in _configuration.Teams.Keys)
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

            var battleRequest = new BattleInitializationRequest(battleParticipants);
            _matchInitializationRequest = new MatchInitializationRequest(battleRequest, _configuration.AgentsByTeam);

            _matchView = _matchRunner.StartMatch(_matchInitializationRequest, _random);
            if (_matchView == null)
            {
                Debug.LogError("Match is not started!");
            }
        }

        private void DisplayStatistics()
        {
            _matchCountText.text = $"{_matchCount} matches";

            var teams = _configuration.Teams.Keys.ToList();
            var firstTeam = teams[0];
            var secondTeam = teams[1];

            var firstTeamWon = _matchWinnerCount[firstTeam.Value];
            var secondTeamWon = _matchWinnerCount[secondTeam.Value];

            _firstAlgorithmText.text = $"{firstTeam.DisplayName} won: {firstTeamWon} ({(firstTeamWon / (float)_currentMatch) * 100:F2}% win rate)";
            _secondAlgorithmText.text = $"{secondTeam.DisplayName} won: {secondTeamWon} ({(secondTeamWon / (float)_currentMatch) * 100:F2}% win rate)";

            _progressBar.fillAmount = _currentMatch / (float)_matchCount;
        }
    }
}
