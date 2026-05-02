using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct BattleParticipantSetup
    {
        public IEntityView Entity { get; }
        public Vector2Int SpawnPosition { get; }
        public TeamId TeamId { get; }
        public IReadOnlyCollection<ICombatActionDefinition> ActionDefinitions { get; }

        public BattleParticipantSetup(IEntityView entity, Vector2Int spawnPosition, TeamId teamId, IReadOnlyCollection<ICombatActionDefinition> actionDefinitions)
        {
            Entity = entity;
            SpawnPosition = spawnPosition;
            TeamId = teamId;
            ActionDefinitions = actionDefinitions;
        }
    }
}