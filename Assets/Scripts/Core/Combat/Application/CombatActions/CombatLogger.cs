using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal class CombatLogger : ICombatLogger
    {
        private readonly IWorldView _worldView;

        public CombatLogger(IWorldView worldView)
        {
            _worldView = worldView;
        }

        public void Log(IEntityView entity, string text)
        {
            var entityLog = GetEntityRepresentation(entity);
            Debug.Log($"[{entityLog}] {text}");
        }

        public string GetEntityRepresentation(IEntityView entity)
        {
            if (!_worldView.TryGetEntityPosition(entity, out var entityPosition))
                return string.Empty;

            var entityId = GetEntityIdString(entity);

            return $"{entityId}, ({entityPosition})";
        }

        public string GetEntityIdString(IEntityView entity)
        {
            var id = entity.Id.ToString().Substring(0, 4);
            return $"{entity.DisplayName}-{id}";
        }
    }
}
