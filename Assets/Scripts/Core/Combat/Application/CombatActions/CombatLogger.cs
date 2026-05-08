using AiAlgorithmsResearch.Core.Combat.Api;
using UnityEngine;
using EntityId = AiAlgorithmsResearch.Core.Entities.Api.EntityId;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal class CombatLogger : ICombatLogger
    {
        private readonly bool _log = false;

        public CombatLogger(bool log = false)
        {
            _log = log;
        }

        public void Log(EntityId entityId, string text, ICombatStateView combatStateView)
        {
            if (combatStateView is not RuntimeCombatState || !_log)
            {
                return;
            }

            var entityLog = GetEntityRepresentation(entityId, combatStateView);
            Debug.Log($"[{entityLog}] {text}");
        }

        public string GetEntityRepresentation(EntityId entityId, ICombatStateView combatStateView)
        {
            if (!combatStateView.TryGetPosition(entityId, out var entityPosition))
                return string.Empty;

            var entityDisplayName = GetEntityDisplayName(entityId);

            return $"{entityDisplayName}, ({entityPosition})";
        }

        public string GetEntityDisplayName(EntityId entityId)
        {
            var id = entityId.ToString().Substring(0, 4);
            // return $"{entity.DisplayName}-{id}";
            return id;
        }
    }
}
