using System;

namespace AiAlgorithmsResearch.Core.Entities.Api
{
    public readonly struct EntityDefinitionId : IEquatable<EntityDefinitionId>
    {
        public string Value { get; }

        public EntityDefinitionId(string value)
        {
            Value = value;
        }

        public bool Equals(EntityDefinitionId other) => Value == other.Value;

        public override bool Equals(object obj)
            => obj is EntityDefinitionId other && Equals(other);

        public override int GetHashCode()
            => Value != null ? Value.GetHashCode() : 0;

        public static bool operator ==(EntityDefinitionId left, EntityDefinitionId right)
            => left.Equals(right);

        public static bool operator !=(EntityDefinitionId left, EntityDefinitionId right)
            => !left.Equals(right);

        public override string ToString() => Value;
    }
}
