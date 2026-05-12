using System;

namespace AiAlgorithmsResearch.Core.Ai.Api
{
    public interface ICombatAgentFactory
    {
        ICombatAgent CreateRandomAgent(Random random);
        ICombatAgent CreateStateMachineAgent();
        ICombatAgent CreateMinimaxAgent(int depth);
        ICombatAgent CreateMonteCarloAgent(int maxIterations, Random random);
    }
}
