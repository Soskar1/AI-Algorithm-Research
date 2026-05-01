using AiAlgorithmsResearch.Core.Ai.Api;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class UnityRandomNumberGenerator : IRandomNumberGenerator
    {
        public int Range(int minInclusive, int maxExclusive)
        {
            return Random.Range(minInclusive, maxExclusive);
        }
    }
}