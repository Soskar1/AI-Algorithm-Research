using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System;
using System.Linq;
using UnityEngine;
using EntityId = AiAlgorithmsResearch.Core.Entities.Api.EntityId;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class RuntimeCombatState : ICombatStateView, ICombatStateEditor
    {
        private readonly IWorldView _worldView;
        private readonly IWorldEditor _worldEditor;
        private readonly IEntityHealthEditor _healthEditor;
        private readonly IEntityEnergyEditor _energyEditor;
        private readonly IActionCooldowns _cooldowns;
        private readonly IActionCooldownEditor _cooldownEditor;
        private readonly IStunStatusEditor _stunEditor;

        public RuntimeCombatState(
            IWorldView worldView,
            IWorldEditor worldEditor,
            IEntityHealthEditor healthEditor,
            IEntityEnergyEditor energyEditor,
            IActionCooldowns cooldowns,
            IActionCooldownEditor cooldownEditor,
            IStunStatusEditor stunEditor)
        {
            _worldView = worldView;
            _worldEditor = worldEditor;
            _healthEditor = healthEditor;
            _energyEditor = energyEditor;
            _cooldowns = cooldowns;
            _cooldownEditor = cooldownEditor;
            _stunEditor = stunEditor;
        }

        public bool TryGetPosition(EntityId entityId, out Vector2Int position)
        {
            foreach (var entity in _worldView.Entities)
            {
                if (entity.Entity.Id.Value == entityId.Value)
                {
                    position = entity.Position;
                    return true;
                }
            }

            position = default;
            return false;
        }

        public bool IsOccupied(Vector2Int position)
        {
            return _worldView.Entities.Any(entity =>
                entity.Entity.Health.Current > 0 &&
                entity.Position == position);
        }

        public bool IsObstacle(Vector2Int position)
        {
            if (!_worldView.Map.TryGetNode(position, out var node))
                return true;

            return node.Type == MapNodeType.Obstacle;
        }

        public int GetHealth(EntityId entityId)
        {
            return GetEntity(entityId).Health.Current;
        }

        public int GetMaxHealth(EntityId entityId)
        {
            return GetEntity(entityId).Health.Max;
        }

        public int GetEnergy(EntityId entityId)
        {
            return GetEntity(entityId).Energy.Current;
        }

        public int GetStrength(EntityId entityId)
        {
            return GetEntity(entityId).Strength;
        }

        public bool IsOnCooldown(EntityId entityId, CombatActionId actionId)
        {
            return _cooldowns.IsOnCooldown(GetEntity(entityId), actionId);
        }

        public bool TryMove(EntityId entityId, Vector2Int position)
        {
            return _worldEditor.TryMoveEntity(GetEntity(entityId), position);
        }

        public bool TrySpendEnergy(EntityId entityId, int amount)
        {
            return _energyEditor.TrySpendEnergy(GetEntity(entityId), amount);
        }

        public void DealDamage(EntityId entityId, int amount)
        {
            _healthEditor.DealDamage(GetEntity(entityId), amount);
        }

        public void Heal(EntityId entityId, int amount)
        {
            _healthEditor.Heal(GetEntity(entityId), amount);
        }

        public void PutOnCooldown(EntityId entityId, CombatActionId actionId, int turns)
        {
            _cooldownEditor.PutOnCooldown(GetEntity(entityId), actionId, turns);
        }

        public void StunForNextTurn(EntityId entityId)
        {
            _stunEditor.StunForNextTurn(GetEntity(entityId));
        }

        private IEntityView GetEntity(EntityId entityId)
        {
            foreach (var worldEntity in _worldView.Entities)
            {
                if (worldEntity.Entity.Id.Value == entityId.Value)
                    return worldEntity.Entity;
            }

            throw new InvalidOperationException($"Entity was not found: {entityId}");
        }
    }
}
