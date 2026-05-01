using UnityEngine;

namespace AiAlgorithmsResearch.Core.Worlds.Api
{
    public static class GridDistance
    {
        public static int Manhattan(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }
}
