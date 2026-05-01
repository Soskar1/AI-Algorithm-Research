using System.Collections.Generic;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Domain
{
    internal sealed class StunStatus : IStunStatus, IStunStatusEditor
    {
        private readonly HashSet<IEntityView> _stunnedEntities = new();

        public bool IsStunned(IEntityView entity)
        {
            return _stunnedEntities.Contains(entity);
        }

        public void StunForNextTurn(IEntityView entity)
        {
            _stunnedEntities.Add(entity);
        }

        public bool ConsumeStun(IEntityView entity)
        {
            return _stunnedEntities.Remove(entity);
        }
    }
}