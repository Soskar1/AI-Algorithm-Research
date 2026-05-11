using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Benchmarks.Domain
{
    internal class BenchmarkConfiguration
    {
        public IReadOnlyList<IReadOnlyDictionary<Vector2Int, EntityDefinitionId>> Teams { get; }
        public IReadOnlyDictionary<EntityDefinitionId, EntityDefinition> EntityDefinitionsById { get; }
        public IReadOnlyDictionary<EntityDefinitionId, IReadOnlyCollection<ICombatActionDefinition>> ActionsByEntity { get; }
        public int WorldWidth { get; }
        public int WorldHeight { get; }
        public List<Vector2Int> Walls { get; }

        public BenchmarkConfiguration(
            IReadOnlyList<IReadOnlyDictionary<Vector2Int, EntityDefinitionId>> teams,
            IReadOnlyDictionary<EntityDefinitionId, EntityDefinition> entityDefinitionsById,
            IReadOnlyDictionary<EntityDefinitionId, IReadOnlyCollection<ICombatActionDefinition>> actionsByEntity,
            int worldWidth,
            int worldHeight,
            List<Vector2Int> walls)
        {
            Teams = teams;
            EntityDefinitionsById = entityDefinitionsById;
            ActionsByEntity = actionsByEntity;
            WorldWidth = worldWidth;
            WorldHeight = worldHeight;
            Walls = walls;
        }
    }
}
