using AiAlgorithmsResearch.Core.Combat.Api;
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
        public int MaxHealth { get; }
        public int Energy { get; set; }
        public int MaxEnergy { get; }
        public int EnergyRegenerationPerTurn { get; }

        public int Strength { get; }
        public int Speed { get; }

        public IDictionary<CombatActionId, int> Cooldowns { get; }
        public bool IsStunned { get; set; }

        public SimulationEntityState(EntityId id, int health, int maxHealth, int energy, int maxEnergy, int strength, int speed, int regenerationPerTurn, TeamId teamId, Vector2Int position, IDictionary<CombatActionId, int> cooldowns, bool isStunned)
        {
            Id = id;
            TeamId = teamId;
            Position = position;
            Health = health;
            MaxHealth = maxHealth;
            Energy = energy;
            EnergyRegenerationPerTurn = regenerationPerTurn;
            MaxEnergy = maxEnergy;
            Strength = strength;
            Speed = speed;
            Cooldowns = cooldowns;
            IsStunned = isStunned;
        }
    }
}
