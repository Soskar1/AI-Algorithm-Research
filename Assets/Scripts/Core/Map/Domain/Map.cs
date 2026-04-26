using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Map.Domain
{
    internal class Map
    {
        private Dictionary<Vector2, MapNode> m_nodes;
        private HashSet<MapEdge> m_edges;

        public bool TryAddNode(Vector2 position, MapNodeType nodeType)
        {
            if (m_nodes.TryGetValue(position, out _))
            {
                return false;
            }

            var node = new MapNode(position, nodeType);
            m_nodes[node.Position] = node;
            return true;
        }

        public bool ChangeNodeType(Vector2 position, MapNodeType newType)
        {
            if (!m_nodes.TryGetValue(position, out var node))
            {
                return false;
            }

            node.SetType(newType);
            return true;
        }

        private bool TryGetEdge(MapNode node1, MapNode node2, out MapEdge edge)
        {
            edge = null;

            var edgeToSearch = new MapEdge(node1, node2);
            if (!m_edges.Contains(edgeToSearch))
            {
                return false;
            }

            edge = edgeToSearch;
            return true;
        }

        public bool ConnectNodes(Vector2 position1, Vector2 position2)
        {
            if (!m_nodes.TryGetValue(position1, out var node1))
            {
                return false;
            }

            if (!m_nodes.TryGetValue(position2, out var node2))
            {
                return false;
            }

            if (TryGetEdge(node1, node2, out _))
            {
                return false;
            }

            var edge = new MapEdge(node1, node2);
            m_edges.Add(edge);
            return true;
        }
    }
}
