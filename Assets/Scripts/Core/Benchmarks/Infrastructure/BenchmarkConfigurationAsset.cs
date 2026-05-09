using AiAlgorithmsResearch.Core.Benchmarks.Domain;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Benchmarks.Infrastructure
{
    [CreateAssetMenu(menuName = "Research/Benchmark Configuration")]
    internal class BenchmarkConfigurationAsset : ScriptableObject
    {
        [SerializeField] private TeamConfiguration[] _teams;
        [SerializeField] private int _worldWidth;
        [SerializeField] private int _worldHeight;
        [SerializeField] private List<Vector2Int> _walls;

        public BenchmarkConfiguration ToConfiguration(string firstTeamDisplayName, string secondTeamDisplayName)
        {
            var teams = new Dictionary<TeamId, IReadOnlyDictionary<Vector2Int, EntityDefinitionId>>();
            var entityDefinitionsById = new Dictionary<EntityDefinitionId, EntityDefinition>();
            var actionsByEntity = new Dictionary<EntityDefinitionId, IReadOnlyCollection<ICombatActionDefinition>>();

            var currentDisplayName = firstTeamDisplayName;
            var currentTeamId = 0;

            foreach (var team in _teams)
            {
                var teamId = new TeamId(currentTeamId, currentDisplayName);
                currentDisplayName = secondTeamDisplayName;
                ++currentTeamId;

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

            return new BenchmarkConfiguration(teams, entityDefinitionsById, actionsByEntity, _worldWidth, _worldHeight, _walls);
        }
    }
}
