using System;

namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public readonly struct TeamId : IEquatable<TeamId>
    {
        public int Value { get; }

        public TeamId(int value)
        {
            Value = value;
        }

        public bool Equals(TeamId other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is TeamId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public static bool operator ==(TeamId left, TeamId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TeamId left, TeamId right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"TeamId({Value})";
        }
    }
}
