using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;
using UnityEngine;
using EntityId = AiAlgorithmsResearch.Core.Entities.Api.EntityId;

namespace AiAlgorithmsResearch.Core.Ai.Domain
{
    internal class SimulationEntityState
    {
        public EntityId Id { get; }
        public TeamId TeamId { get; }
        public Vector2Int Position { get; set; }

        public int Health { get; set; }
        public int Energy { get; set; }

        public int Strength { get; }
        public int Speed { get; }

        public IDictionary<CombatActionId, int> Cooldowns { get; }
        public bool IsStunned { get; set; }

        public SimulationEntityState(IEntityView entityView, TeamId teamId, Vector2Int position, IDictionary<CombatActionId, int> cooldowns, bool isStunned)
        {
            Id = entityView.Id;
            TeamId = teamId;
            Position = position;
            Health = entityView.Health.Current;
            Energy = entityView.Energy.Current;
            Strength = entityView.Strength;
            Speed = entityView.Speed;
            Cooldowns = cooldowns;
            IsStunned = isStunned;
        }
    }
}
