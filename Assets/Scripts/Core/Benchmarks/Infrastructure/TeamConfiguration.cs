using System;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Benchmarks.Infrastructure
{
    [Serializable]
    internal sealed class TeamConfiguration
    {
        [SerializeField] private int _teamId;
        [SerializeField] private EntitySpawnConfiguration[] _entities;

        public int TeamId => _teamId;
        public IReadOnlyCollection<EntitySpawnConfiguration> Entities => _entities;
    }
}
