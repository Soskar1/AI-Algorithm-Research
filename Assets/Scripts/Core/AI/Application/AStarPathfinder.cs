using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Worlds.Api;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal static class AStarPathfinder
    {
        private static readonly Vector2Int[] Directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        private sealed class Node
        {
            public Vector2Int Position { get; }
            public Node Parent { get; set; }
            public int PathCost { get; set; }
            public int HeuristicCost { get; set; }
            public int TotalCost => PathCost + HeuristicCost;

            public Node(Vector2Int position, int pathCost, int heuristicCost, Node parent = null)
            {
                Position = position;
                PathCost = pathCost;
                HeuristicCost = heuristicCost;
                Parent = parent;
            }
        }

        public static IReadOnlyList<Vector2Int> FindPath(ICombatStateView combatState, Vector2Int start, Vector2Int target)
        {
            if (start == target)
            {
                return null;
            }

            if (!Targeting.IsValidDestination(combatState, target))
            {
                return null;
            }

            var startNode = new Node(start, pathCost: 0, heuristicCost: GridDistance.Manhattan(start, target));

            var visited = new HashSet<Vector2Int>();
            var nodesInProcess = new Dictionary<Vector2Int, Node>
            {
                [start] = startNode
            };

            var queue = new PriorityQueue<Node, int>();
            queue.Enqueue(startNode, startNode.TotalCost);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (visited.Contains(current.Position))
                {
                    continue;
                }

                if (current.Position == target)
                {
                    return ReconstructPath(current);
                }

                visited.Add(current.Position);

                foreach (var direction in Directions)
                {
                    var adjacent = current.Position + direction;

                    if (visited.Contains(adjacent) || !Targeting.IsValidDestination(combatState, adjacent))
                    {
                        continue;
                    }

                    var pathCost = current.PathCost + 1;

                    if (!nodesInProcess.TryGetValue(adjacent, out var existing))
                    {
                        var node = new Node(adjacent, pathCost, GridDistance.Manhattan(adjacent, target), current);

                        nodesInProcess[adjacent] = node;
                        queue.Enqueue(node, node.TotalCost);
                    }
                    else if (pathCost < existing.PathCost)
                    {
                        existing.Parent = current;
                        existing.PathCost = pathCost;
                        existing.HeuristicCost = GridDistance.Manhattan(adjacent, target);

                        queue.Enqueue(existing, existing.TotalCost);
                    }
                }
            }

            return null;
        }

        private static IReadOnlyList<Vector2Int> ReconstructPath(Node current)
        {
            var path = new List<Vector2Int>();

            while (current != null)
            {
                path.Add(current.Position);
                current = current.Parent;
            }

            path.Reverse();
            return path;
        }
    }
}
