using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Benchmarks.Infrastructure
{
    [Serializable]
    internal sealed class EntitySpawnConfiguration
    {
        [SerializeField] private Vector2Int _position;
        [SerializeField] private BattleParticipantAsset _battleParticipant;

        public Vector2Int Position => _position;
        public EntityDefinitionAsset EntityDefinitionAsset => _battleParticipant.EntityDefinition;
        public IReadOnlyCollection<CombatActionDefinitionAsset> ActionDefinitions => _battleParticipant.CombatActions;
    }
}
