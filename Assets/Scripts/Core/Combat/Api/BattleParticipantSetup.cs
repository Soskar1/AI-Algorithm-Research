using AiAlgorithmsResearch.Core.Entities.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct BattleParticipantSetup
    {
        public IEntityView Entity { get; }
        public Vector2Int SpawnPosition { get; }
        public TeamId TeamId { get; }

        public BattleParticipantSetup(IEntityView entity, Vector2Int spawnPosition, TeamId teamId)
        {
            Entity = entity;
            SpawnPosition = spawnPosition;
            TeamId = teamId;
        }
    }
}