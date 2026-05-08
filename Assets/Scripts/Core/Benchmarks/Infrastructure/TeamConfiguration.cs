using System;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Benchmarks.Infrastructure
{
    [Serializable]
    internal sealed class TeamConfiguration
    {
        [SerializeField] private int _teamId;
        [SerializeField] private string _displayName;
        [SerializeField] private CombatAgentType _agentType;
        [SerializeField] private EntitySpawnConfiguration[] _entities;

        public int TeamId => _teamId;
        public CombatAgentType AgentType => _agentType;
        public IReadOnlyCollection<EntitySpawnConfiguration> Entities => _entities;
        public string DisplayName => _displayName;
    }
}
