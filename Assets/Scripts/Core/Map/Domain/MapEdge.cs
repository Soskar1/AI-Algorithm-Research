using System;

namespace AiAlgorithmsResearch.Core.Map.Domain
{
    internal sealed class MapEdge : IEquatable<MapEdge>
    {
        public MapNode First { get; }
        public MapNode Second { get; }

        public MapEdge(MapNode a, MapNode b)
        {
            if (a.GetHashCode() <= b.GetHashCode())
            {
                First = a;
                Second = b;
            }
            else
            {
                First = b;
                Second = a;
            }
        }

        public bool Equals(MapEdge other)
        {
            if (other is null)
            {
                return false;
            }

            return First.Equals(other.First) && Second.Equals(other.Second);
        }

        public override bool Equals(object obj) => obj is MapEdge other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(First, Second);

        public static bool operator ==(MapEdge left, MapEdge right) => Equals(left, right);
        public static bool operator !=(MapEdge left, MapEdge right) => !Equals(left, right);
    }
}
