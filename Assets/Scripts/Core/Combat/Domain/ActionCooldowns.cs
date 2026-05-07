using System.Collections.Generic;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Domain
{
    internal sealed class ActionCooldowns : IActionCooldowns, IActionCooldownEditor
    {
        private readonly Dictionary<EntityId, Dictionary<CombatActionId, int>> _cooldowns = new();

        public bool IsOnCooldown(EntityId entity, CombatActionId actionId)
        {
            return GetRemainingCooldown(entity, actionId) > 0;
        }

        public int GetRemainingCooldown(EntityId entity, CombatActionId actionId)
        {
            if (!_cooldowns.TryGetValue(entity, out var entityCooldowns))
                return 0;

            if (!entityCooldowns.TryGetValue(actionId, out var remaining))
                return 0;

            return remaining;
        }

        public void PutOnCooldown(EntityId entity, CombatActionId actionId, int turns)
        {
            if (turns <= 0)
                return;

            if (!_cooldowns.TryGetValue(entity, out var entityCooldowns))
            {
                entityCooldowns = new Dictionary<CombatActionId, int>();
                _cooldowns[entity] = entityCooldowns;
            }

            entityCooldowns[actionId] = turns;
        }

        public void TickCooldowns(EntityId entity)
        {
            if (!_cooldowns.TryGetValue(entity, out var entityCooldowns))
                return;

            var actionIds = new List<CombatActionId>(entityCooldowns.Keys);

            foreach (var actionId in actionIds)
            {
                entityCooldowns[actionId]--;

                if (entityCooldowns[actionId] <= 0)
                {
                    entityCooldowns.Remove(actionId);
                }
            }
        }

        public IDictionary<CombatActionId, int> CopyEntityCooldowns(EntityId entity)
        {
            if (_cooldowns.TryGetValue(entity, out var cooldowns))
            {
                return new Dictionary<CombatActionId, int>(cooldowns);
            }

            return new Dictionary<CombatActionId, int>();
        }
    }
}