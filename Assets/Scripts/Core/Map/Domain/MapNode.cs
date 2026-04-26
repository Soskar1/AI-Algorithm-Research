using System;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Map.Domain
{
    internal struct MapNode : IEquatable<MapNode>
    {
        public Vector2 Position { get; }
        public MapNodeType Type { get; private set; }

        public MapNode(Vector2 position, MapNodeType type)
        {
            Position = position;
            Type = type;
        }

        public void SetType(MapNodeType type) => Type = type;
        public bool Equals(MapNode other) => Position.Equals(other.Position);
        public override bool Equals(object obj) => obj is MapNode other && Equals(other);
        public override int GetHashCode() => Position.GetHashCode();

        public static bool operator ==(MapNode left, MapNode right) => left.Equals(right);
        public static bool operator !=(MapNode left, MapNode right) => !left.Equals(right);
    }
}