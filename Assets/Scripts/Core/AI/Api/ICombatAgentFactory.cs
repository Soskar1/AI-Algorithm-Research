using AiAlgorithmsResearch.Core.Maps.Api;

namespace AiAlgorithmsResearch.Core.Ai.Api
{
    public interface ICombatAgentFactory
    {
        ICombatAgent CreateRandomAgent();
    }
}
