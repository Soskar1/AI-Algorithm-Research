using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Maps.Api;

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

        public ICombatAgent CreateRandomAgent()
        {
            var simulationFactory = new SimulationStateFactory(_map);
            return new RandomCombatAgent(simulationFactory, _provider, _executor);
        }
    }
}
