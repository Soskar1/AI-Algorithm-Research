using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class RandomCombatAgent : ICombatAgent
    {
        private readonly IRandomNumberGenerator _random;

        public RandomCombatAgent(IRandomNumberGenerator random)
        {
            _random = random;
        }

        public ICombatAction ChooseAction(IList<ICombatAction> actions)
        {
            var index = _random.Range(0, actions.Count);

            return actions[index];
        }
    }
}