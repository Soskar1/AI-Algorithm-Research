using System.Collections.Generic;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Domain
{
    internal sealed class StunStatus : IStunStatus, IStunStatusEditor
    {
        private readonly HashSet<EntityId> _stunnedEntities = new();

        public bool IsStunned(EntityId entity)
        {
            return _stunnedEntities.Contains(entity);
        }

        public void StunForNextTurn(EntityId entity)
        {
            _stunnedEntities.Add(entity);
        }

        public bool ConsumeStun(EntityId entity)
        {
            return _stunnedEntities.Remove(entity);
        }
    }
}