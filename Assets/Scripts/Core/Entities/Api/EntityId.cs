using System;

namespace AiAlgorithmsResearch.Core.Entities.Api
{
    public readonly struct EntityId
    {
        public Guid Value { get; }

        public EntityId(Guid value)
        {
            Value = value;
        }
    }
}
