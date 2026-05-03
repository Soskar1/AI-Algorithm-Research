using AiAlgorithmsResearch.Core.Benchmarks.Domain;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Matches.Api;
using Reflex.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Benchmarks.Infrastructure
{
    internal class BenchmarkRunner : MonoBehaviour
    {
        private IMatchRunner _matchRunner;
        private IEntityFactory _entityFactory;
        private BenchmarkConfiguration _configuration;
        private MatchInitializationRequest _matchInitializationRequest;

        [Inject]
        public void Inject(IMatchRunner matchRunner, IEntityFactory entityFactory)
        {
            _matchRunner = matchRunner;
            _entityFactory = entityFactory;
        }

        public void Start()
        {
            List<BattleParticipantSetup> battleParticipants = new();
            var teams = _configuration.Teams.Keys;
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
        }

        public void Update()
        {
            // _matchRunner.StartMatch(_matchInitializationRequest);
            // _matchRunner.Tick();
        }
    }
}
