
using System;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct CombatActionId : IEquatable<CombatActionId>
    {
        public string Value { get; }

        public CombatActionId(string value)
        {
            Value = value;
        }

        public bool Equals(CombatActionId other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is CombatActionId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }

        public static bool operator ==(CombatActionId left, CombatActionId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CombatActionId left, CombatActionId right)
        {
            return !left.Equals(right);
        }
    }
}