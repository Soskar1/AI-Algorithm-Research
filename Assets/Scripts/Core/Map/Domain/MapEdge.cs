using System;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Maps.Domain
{
    internal sealed class MapEdge : IEquatable<MapEdge>
    {
        public Vector2Int First { get; }
        public Vector2Int Second { get; }

        public MapEdge(Vector2Int first, Vector2Int second)
        {
            if (Compare(first, second) <= 0)
            {
                First = first;
                Second = second;
            }
            else
            {
                First = second;
                Second = first;
            }
        }

        private static int Compare(Vector2Int x, Vector2Int y)
        {
            var cx = x.x.CompareTo(y.x);
            return cx != 0 ? cx : x.y.CompareTo(y.y);
        }

        public bool Equals(MapEdge other)
        {
            if (other is null) return false;
            return First.Equals(other.First) && Second.Equals(other.Second);
        }

        public override bool Equals(object obj) => obj is MapEdge other && Equals(other);

        public override int GetHashCode()
        {
            return HashCode.Combine(First, Second);
        }
    }
}
