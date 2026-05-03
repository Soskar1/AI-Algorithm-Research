using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Benchmarks.Domain;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Matches.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using Reflex.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Benchmarks.Infrastructure
{
    internal class BenchmarkRunner : MonoBehaviour
    {
        [SerializeField] private BenchmarkConfigurationAsset _benchmarkConfigurationAsset;
        [SerializeField] private int _worldWidth;
        [SerializeField] private int _worldHeight;

        private IMatchRunner _matchRunner;
        private IEntityFactory _entityFactory;
        private ICombatAgentFactory _combatAgentFactory;
        private IWorldGenerator _worldGenerator;
        private BenchmarkConfiguration _configuration;
        private MatchInitializationRequest _matchInitializationRequest;

        private IMatchView _matchView;

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
            _configuration = _benchmarkConfigurationAsset.ToConfiguration(_combatAgentFactory);
            var teams = _configuration.Teams.Keys;

            List<BattleParticipantSetup> battleParticipants = new();
            foreach (var team in teams)
            {
                var teamEntities = _configuration.Teams[team];

                foreach ((var position, var entityDefinitionId) in teamEntities) {
                    var entityDefinition = _configuration.EntityDefinitionsById[entityDefinitionId];
                    var entity = _entityFactory.CreateEntity(entityDefinition);
                    var actionDefinitions = _configuration.ActionsByEntity[entityDefinitionId];
                    var battleParticipantSetup = new BattleParticipantSetup(entity, position, team, actionDefinitions);

                    battleParticipants.Add(battleParticipantSetup);
                }
            }

            var battleRequest = new BattleInitializationRequest(battleParticipants);
            _matchInitializationRequest = new MatchInitializationRequest(battleRequest, _configuration.AgentsByTeam);

            _worldGenerator.Generate(_worldWidth, _worldHeight);

            _matchView = _matchRunner.StartMatch(_matchInitializationRequest);
            if (_matchView == null)
            {
                Debug.LogError("Match is not started!");
            }
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
                Debug.Log($"Match ended. Winner: {_matchView.Winner}");
            }
        }
    }
}
