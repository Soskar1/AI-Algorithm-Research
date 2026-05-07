using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Maps.Api;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class CombatAgentFactory : ICombatAgentFactory
    {
        private readonly CombatActionCandidateProvider _provider;
        private readonly IReadOnlyTileMap _map;

        public CombatAgentFactory(CombatActionCandidateProvider provider, IReadOnlyTileMap map)
        {
            _provider = provider;
            _map = map;
        }

        public ICombatAgent CreateRandomAgent()
        {
            var simulationFactory = new SimulationStateFactory(_map);
            return new RandomCombatAgent(simulationFactory, _provider);
        }
    }
}
