using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System;
using System.Collections.Generic;
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
        private readonly IStunStatus _stunStatus;
        private readonly IBattle _battle;

        private readonly List<EntityId> _entityIds;
        public IReadOnlyCollection<EntityId> EntityIds => _entityIds;

        private readonly Dictionary<EntityId, IReadOnlyCollection<ICombatActionDefinition>> _actionDefinitions;

        public RuntimeCombatState(
            IWorldView worldView,
            IWorldEditor worldEditor,
            IEntityHealthEditor healthEditor,
            IEntityEnergyEditor energyEditor,
            IActionCooldowns cooldowns,
            IActionCooldownEditor cooldownEditor,
            IStunStatusEditor stunEditor,
            IStunStatus stunStatus,
            IBattle battle)
        {
            _worldView = worldView;
            _worldEditor = worldEditor;
            _healthEditor = healthEditor;
            _energyEditor = energyEditor;
            _cooldowns = cooldowns;
            _cooldownEditor = cooldownEditor;
            _stunEditor = stunEditor;
            _stunStatus = stunStatus;
            _battle = battle;

            _entityIds = battle.EntityTeams.Keys.ToList();
            _actionDefinitions = new Dictionary<EntityId, IReadOnlyCollection<ICombatActionDefinition>>();

            foreach (var battleParticipant in battle.TurnOrder)
            {
                var id = battleParticipant.Entity.Id;
                var actions = battleParticipant.ActionDefinitions;

                _actionDefinitions.Add(id, actions);
            }
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
            return _cooldowns.IsOnCooldown(entityId, actionId);
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
            _cooldownEditor.PutOnCooldown(entityId, actionId, turns);
        }

        public void StunForNextTurn(EntityId entityId)
        {
            _stunEditor.StunForNextTurn(entityId);
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

        public TeamId GetTeamId(EntityId entityId)
        {
            return _battle.EntityTeams[entityId];
        }

        public IReadOnlyCollection<ICombatActionDefinition> GetCombatActionDefinitions(EntityId entityId)
        {
            return _actionDefinitions[entityId];
        }

        public int GetSpeed(EntityId entityId)
        {
            return GetEntity(entityId).Speed;
        }

        public bool IsStunned(EntityId entityId)
        {
            return _stunStatus.IsStunned(entityId);
        }

        public IDictionary<CombatActionId, int> GetCooldowns(EntityId entityId)
        {
            return _cooldowns.CopyEntityCooldowns(entityId);
        }
    }
}
