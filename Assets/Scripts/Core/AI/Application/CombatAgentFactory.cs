using AiAlgorithmsResearch.Core.Ai.Api;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class CombatAgentFactory : ICombatAgentFactory
    {
        public ICombatAgent CreateRandomAgent()
        {
            return new RandomCombatAgent(new UnityRandomNumberGenerator());
        }
    }
}
