using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class UnityInitiativeRoller : IInitiativeRoller
    {
        public int Roll(IEntityView entity)
        {
            return UnityEngine.Random.Range(1, 21);
        }
    }
}
