using System.Linq;
using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class RandomCombatAgent : ICombatAgent
    {
        private readonly IRandomNumberGenerator _random;

        public RandomCombatAgent(IRandomNumberGenerator random)
        {
            _random = random;
        }

        public ICombatAction ChooseAction(CombatAgentContext context)
        {
            if (context.AvailableActions == null || context.AvailableActions.Count == 0)
            {
                return new WaitAction(context.Actor);
            }

            var index = _random.Range(0, context.AvailableActions.Count);

            return context.AvailableActions.ElementAt(index);
        }
    }
}