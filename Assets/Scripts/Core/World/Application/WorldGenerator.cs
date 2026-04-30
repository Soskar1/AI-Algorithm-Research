using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Worlds.Domain;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Worlds.Application
{
    internal sealed class WorldGenerator
    {
        private readonly World _world;

        public WorldGenerator(World world)
        {
            _world = world;
        }

        public void GenerateRectangle(int width, int height)
        {
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var position = new Vector2Int(x, y);
                    var type = IsBorder(x, y, width, height) ? MapNodeType.Obstacle : MapNodeType.Free;

                    _world.AddTile(position, type);
                }
            }

            ConnectOrthogonalNeighbors(width, height);
        }

        private void ConnectOrthogonalNeighbors(int width, int height)
        {
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var current = new Vector2Int(x, y);

                    if (x + 1 < width)
                    {
                        _world.Connect(current, new Vector2Int(x + 1, y));
                    }

                    if (y + 1 < height)
                    {
                        _world.Connect(current, new Vector2Int(x, y + 1));
                    }
                }
            }
        }

        private static bool IsBorder(int x, int y, int width, int height)
        {
            return x == 0
                   || y == 0
                   || x == width - 1
                   || y == height - 1;
        }
    }
}
