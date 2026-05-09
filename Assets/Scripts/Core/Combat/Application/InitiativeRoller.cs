using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System;

namespace AiAlgorithmsResearch.Core.Combat.Application
{
    internal sealed class InitiativeRoller : IInitiativeRoller
    {
        private readonly Random _random;

        public InitiativeRoller(Random random)
        {
            _random = random;
        }

        public int Roll(IEntityView entity)
        {
            return _random.Next(1, 21);
        }
    }
}
