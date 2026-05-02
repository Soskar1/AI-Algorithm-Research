using AiAlgorithmsResearch.Core.Combat.Domain;
using AiAlgorithmsResearch.Core.Entities.Api;
using System;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class EntityCombatActions
    {
        private readonly Dictionary<EntityId, IReadOnlyCollection<ICombatActionDefinition>> _actionsByEntity = new();

        public void SetActions(IEntityView entity, IReadOnlyCollection<ICombatActionDefinition> actions)
        {
            _actionsByEntity[entity.Id] = actions;
        }

        public IReadOnlyCollection<ICombatActionDefinition> GetActions(IEntityView entity)
        {
            return _actionsByEntity.TryGetValue(entity.Id, out var actions)
                ? actions
                : Array.Empty<ICombatActionDefinition>();
        }
    }
}
