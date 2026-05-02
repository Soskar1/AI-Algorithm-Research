using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Benchmarks.Domain
{
    internal class BenchmarkConfiguration
    {
        public IReadOnlyDictionary<TeamId, IReadOnlyDictionary<Vector2Int, EntityDefinition>> Teams { get; }
        public IReadOnlyDictionary<TeamId, ICombatAgent> AgentsByTeam { get; }
        public IReadOnlyDictionary<EntityDefinition, IReadOnlyCollection<ICombatActionDefinition>> ActionsByEntity { get; }
    }
}
