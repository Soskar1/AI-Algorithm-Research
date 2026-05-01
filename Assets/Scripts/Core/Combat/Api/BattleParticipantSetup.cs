using AiAlgorithmsResearch.Core.Entities.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct BattleParticipantSetup
    {
        public IEntityView Entity { get; }
        public Vector2Int SpawnPosition { get; }

        public BattleParticipantSetup(IEntityView entity, Vector2Int spawnPosition)
        {
            Entity = entity;
            SpawnPosition = spawnPosition;
        }
    }
}