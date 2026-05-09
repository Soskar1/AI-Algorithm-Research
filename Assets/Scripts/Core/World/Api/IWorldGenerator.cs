using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Worlds.Api
{
    public interface IWorldGenerator
    {
        public void Generate(int width, int height, IReadOnlyCollection<Vector2Int> obstacles);
    }
}
