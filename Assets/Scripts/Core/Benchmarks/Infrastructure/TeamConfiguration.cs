using System;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Benchmarks.Infrastructure
{
    [Serializable]
    internal sealed class TeamConfiguration
    {
        [SerializeField] private EntitySpawnConfiguration[] _entities;

        public IReadOnlyCollection<EntitySpawnConfiguration> Entities => _entities;
    }
}
