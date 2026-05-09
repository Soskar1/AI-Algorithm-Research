using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Benchmarks.Domain;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace AiAlgorithmsResearch.Core.Benchmarks.Infrastructure
{
    [CreateAssetMenu(menuName = "Research/Benchmark Configuration")]
    internal class BenchmarkConfigurationAsset : ScriptableObject
    {
        [SerializeField] private TeamConfiguration[] _teams;
        [SerializeField] private int _worldWidth;
        [SerializeField] private int _worldHeight;
        [SerializeField] private List<Vector2Int> _walls;

        public TeamConfiguration[] Teams => _teams;

        public BenchmarkConfiguration ToConfiguration(ICombatAgentFactory agentFactory, Random random)
        {
            var teams = new Dictionary<TeamId, IReadOnlyDictionary<Vector2Int, EntityDefinitionId>>();
            var entityDefinitionsById = new Dictionary<EntityDefinitionId, EntityDefinition>();
            var actionsByEntity = new Dictionary<EntityDefinitionId, IReadOnlyCollection<ICombatActionDefinition>>();
            var agentsByTeam = new Dictionary<TeamId, ICombatAgent>();

            foreach (var team in _teams)
            {
                var teamId = new TeamId(team.TeamId, team.DisplayName);

                agentsByTeam[teamId] = team.AgentType switch
                {
                    CombatAgentType.Random => agentFactory.CreateRandomAgent(random),
                    CombatAgentType.StateMachine => agentFactory.CreateStateMachineAgent(),
                    _ => throw new ArgumentOutOfRangeException()
                };

                var entitiesByPosition = new Dictionary<Vector2Int, EntityDefinitionId>();

                foreach (var entitySetup in team.Entities)
                {
                    var entityDefinitionAsset = entitySetup.EntityDefinitionAsset;
                    var entityId = entityDefinitionAsset.Id;

                    entitiesByPosition[entitySetup.Position] = entityId;
                    entityDefinitionsById[entityId] = entityDefinitionAsset.ToDefinition();

                    actionsByEntity[entityId] = entitySetup.ActionDefinitions
                        .Where(action => action != null)
                        .Select(action => action.ToCombatActionDefinition())
                        .ToArray();
                }

                teams[teamId] = entitiesByPosition;
            }

            return new BenchmarkConfiguration(teams, entityDefinitionsById, actionsByEntity, agentsByTeam, _worldWidth, _worldHeight, _walls);
        }
    }
}
