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

                foreach ((var position, var entityDefinition) in teamEntities) {
                    var entity = _entityFactory.CreateEntity(entityDefinition);
                    var actionDefinitions = _configuration.ActionsByEntity[entityDefinition];
                    var battleParticipantSetup = new BattleParticipantSetup(entity, position, team, actionDefinitions);

                    battleParticipants.Add(battleParticipantSetup);
                }
            }

            // TODO: Maybe we need to add here ICombatAgent factory

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
