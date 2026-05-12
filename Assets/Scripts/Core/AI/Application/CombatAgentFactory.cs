using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Maps.Api;
using System;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class CombatAgentFactory : ICombatAgentFactory
    {
        private readonly CombatActionCandidateProvider _provider;
        private readonly IReadOnlyTileMap _map;
        private readonly ICombatActionExecutor _executor;

        public CombatAgentFactory(CombatActionCandidateProvider provider, IReadOnlyTileMap map, ICombatActionExecutor executor)
        {
            _provider = provider;
            _map = map;
            _executor = executor;
        }

        public ICombatAgent CreateRandomAgent(Random random)
        {
            var simulationFactory = new SimulationStateFactory(_map);
            return new RandomCombatAgent(simulationFactory, _provider, _executor, random);
        }

        public ICombatAgent CreateStateMachineAgent()
        {
            var simulationFactory = new SimulationStateFactory(_map);
            return new StateMachineCombatAgent(simulationFactory, _provider, _executor);
        }

        public ICombatAgent CreateMinimaxAgent(int depth)
        {
            var simulationFactory = new SimulationStateFactory(_map);
            return new MinimaxCombatAgent(simulationFactory, _provider, _executor, depth);
        }

        public ICombatAgent CreateMonteCarloAgent(int maxIterations, Random random)
        {
            var simulationFactory = new SimulationStateFactory(_map);
            return new MonteCarloAgent(simulationFactory, _provider, _executor, maxIterations, random);
        }
    }
}
