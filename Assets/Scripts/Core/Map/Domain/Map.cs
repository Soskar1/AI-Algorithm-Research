using AiAlgorithmsResearch.Core.Maps.Api;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Maps.Domain
{
    internal class Map : IReadOnlyTileMap
    {
        private readonly Dictionary<Vector2Int, MapNode> _nodes = new();
        private readonly HashSet<MapEdge> _edges = new();

        public bool TryAddNode(Vector2Int position, MapNodeType nodeType)
        {
            if (_nodes.TryGetValue(position, out _))
            {
                return false;
            }

            var node = new MapNode(position, nodeType);
            _nodes[node.Position] = node;
            return true;
        }

        public bool ChangeNodeType(Vector2Int position, MapNodeType newType)
        {
            if (!_nodes.TryGetValue(position, out var node))
            {
                return false;
            }

            node.SetType(newType);
            return true;
        }

        private bool TryGetEdge(MapNode node1, MapNode node2, out MapEdge edge)
        {
            edge = null;

            var edgeToSearch = new MapEdge(node1.Position, node2.Position);
            if (!_edges.Contains(edgeToSearch))
            {
                return false;
            }

            edge = edgeToSearch;
            return true;
        }

        public bool ConnectNodes(Vector2Int position1, Vector2Int position2)
        {
            if (!_nodes.TryGetValue(position1, out var node1))
            {
                return false;
            }

            if (!_nodes.TryGetValue(position2, out var node2))
            {
                return false;
            }

            if (TryGetEdge(node1, node2, out _))
            {
                return false;
            }

            var edge = new MapEdge(node1.Position, node2.Position);
            _edges.Add(edge);
            return true;
        }

        public bool TryGetNode(Vector2Int position, out MapNodeReadOnly readonlyNode)
        {
            readonlyNode = null;

            if (!_nodes.TryGetValue(position, out var internalNode))
            {
                return false;
            }

            readonlyNode = new MapNodeReadOnly(internalNode.Position, internalNode.Type);
            return true;
        }
    }
}
