using UnityEngine;

namespace AiAlgorithmsResearch.Core.Maps.Api
{
    public class MapNodeReadOnly
    {
        public Vector2Int Position { get; }
        public MapNodeType Type { get; }

        public MapNodeReadOnly(Vector2Int position, MapNodeType nodeType)
        {
            Position = position;
            Type = nodeType;
        }
    }
}
