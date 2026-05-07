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

        public SimulationCombatState(
            IDictionary<EntityId, SimulationEntityState> entities,
            IDictionary<EntityId, IReadOnlyCollection<ICombatActionDefinition>> actions,
            IReadOnlyTileMap map
            )
        {
            _entities = entities.Keys.ToList();
            _simulationEntities = entities;
            _map = map;
            _actions = actions;
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
    }
}
