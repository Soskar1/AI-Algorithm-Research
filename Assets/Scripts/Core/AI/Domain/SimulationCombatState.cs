using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Maps.Api;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EntityId = AiAlgorithmsResearch.Core.Entities.Api.EntityId;

namespace AiAlgorithmsResearch.Core.Ai.Domain
{
    internal class SimulationCombatState : ICombatStateView, ICombatStateEditor
    {
        private readonly IReadOnlyTileMap _map;
        private readonly List<EntityId> _entities;
        private readonly IDictionary<EntityId, IReadOnlyCollection<ICombatActionDefinition>> _actions;
        private readonly IDictionary<EntityId, SimulationEntityState> _simulationEntities;

        public IReadOnlyCollection<EntityId> EntityIds => _entities;

        public EntityId CurrentEntityTurn => TurnOrder[_currentEntityIndex];
        public IReadOnlyList<EntityId> TurnOrder { get; }
        private int _currentEntityIndex = 0;

        public SimulationCombatState(
            IDictionary<EntityId, SimulationEntityState> entities,
            IDictionary<EntityId, IReadOnlyCollection<ICombatActionDefinition>> actions,
            IReadOnlyTileMap map,
            IReadOnlyList<EntityId> turnOrder,
            EntityId currentEntityTurn
            )
        {
            _entities = entities.Keys.ToList();
            _simulationEntities = entities;
            _map = map;
            _actions = actions;

            TurnOrder = turnOrder;

            for (int i = 0; i < turnOrder.Count; ++i)
            {
                var entity = turnOrder[i];
                if (entity.Equals(currentEntityTurn))
                {
                    _currentEntityIndex = i;
                    break;
                }
            }
        }

        public bool TryGetPosition(EntityId entityId, out Vector2Int position)
        {
            if (!_simulationEntities.TryGetValue(entityId, out var entity))
            {
                position = default;
                return false;
            }

            position = entity.Position;
            return true;
        }

        public bool IsOccupied(Vector2Int position)
        {
            return _simulationEntities.Values.Any(entity =>
                entity.Health > 0 &&
                entity.Position == position);
        }

        public bool IsObstacle(Vector2Int position)
        {
            if (!_map.TryGetNode(position, out var node))
            {
                return false;
            }

            return node.Type == MapNodeType.Obstacle;
        }

        public int GetHealth(EntityId entityId)
        {
            return _simulationEntities[entityId].Health;
        }

        public int GetMaxHealth(EntityId entityId)
        {
            return _simulationEntities[entityId].MaxHealth;
        }

        public int GetEnergy(EntityId entityId)
        {
            return _simulationEntities[entityId].Energy;
        }

        public int GetStrength(EntityId entityId)
        {
            return _simulationEntities[entityId].Strength;
        }

        public bool IsOnCooldown(EntityId entityId, CombatActionId actionId)
        {
            return _simulationEntities[entityId].Cooldowns.TryGetValue(actionId, out var cooldown) && cooldown > 0;
        }

        public void TickCooldowns(EntityId entityId)
        {
            var cooldowns = _simulationEntities[entityId].Cooldowns;
            var cooldownsToRemove = new List<CombatActionId>();

            foreach (var cooldown in cooldowns)
            {
                var newCooldown = cooldown.Value - 1;
                cooldowns[cooldown.Key] = newCooldown;

                if (newCooldown <= 0)
                {
                    cooldownsToRemove.Add(cooldown.Key);
                }
            }

            foreach (var cooldown in cooldownsToRemove)
            {
                cooldowns.Remove(cooldown);
            }
        }

        public void RegenerateEnergy(EntityId entityId)
        {
            var regenerationPerTurn = GetEnergyRegenerationPerTurn(entityId);
            var maxEnergy = GetMaxEnergy(entityId);
            var energy = GetEnergy(entityId);
            _simulationEntities[entityId].Energy = Mathf.Clamp(energy + regenerationPerTurn, 0, maxEnergy);
        }

        public bool TryMove(EntityId entityId, Vector2Int position)
        {
            if (IsOccupied(position))
                return false;

            _simulationEntities[entityId].Position = position;
            return true;
        }

        public bool TrySpendEnergy(EntityId entityId, int amount)
        {
            var entity = _simulationEntities[entityId];

            if (entity.Energy < amount)
                return false;

            entity.Energy -= amount;
            return true;
        }

        public void DealDamage(EntityId entityId, int amount)
        {
            var entity = _simulationEntities[entityId];
            entity.Health = Mathf.Max(0, entity.Health - amount);
        }

        public void Heal(EntityId entityId, int amount)
        {
            var entity = _simulationEntities[entityId];
            entity.Health = Mathf.Min(entity.MaxHealth, entity.Health + amount);
        }

        public void PutOnCooldown(EntityId entityId, CombatActionId actionId, int turns)
        {
            _simulationEntities[entityId].Cooldowns[actionId] = turns;
        }

        public void StunForNextTurn(EntityId entityId)
        {
            _simulationEntities[entityId].IsStunned = true;
        }

        public TeamId GetTeamId(EntityId entityId)
        {
            return _simulationEntities[entityId].TeamId;
        }

        public bool AreFriends(EntityId firstEntity, EntityId secondEntity)
        {
            var firstSimulationEntity = _simulationEntities[firstEntity];
            var secondSimulationEntity = _simulationEntities[secondEntity];

            return firstSimulationEntity.TeamId == secondSimulationEntity.TeamId;
        }

        public IReadOnlyCollection<ICombatActionDefinition> GetCombatActionDefinitions(EntityId entityId)
        {
            return _actions[entityId];
        }

        public int GetSpeed(EntityId entityId)
        {
            return _simulationEntities[entityId].Speed;
        }

        public bool IsStunned(EntityId entityId)
        {
            return _simulationEntities[entityId].IsStunned;
        }

        public IDictionary<CombatActionId, int> GetCooldowns(EntityId entityId)
        {
            return new Dictionary<CombatActionId, int>(_simulationEntities[entityId].Cooldowns);
        }

        public void NextTurn()
        {
            _currentEntityIndex = (_currentEntityIndex + 1) % TurnOrder.Count;
        }

        public int GetEnergyRegenerationPerTurn(EntityId entityId)
        {
            return _simulationEntities[entityId].EnergyRegenerationPerTurn;
        }

        public int GetMaxEnergy(EntityId entityId)
        {
            return _simulationEntities[entityId].MaxEnergy;
        }
    }
}
