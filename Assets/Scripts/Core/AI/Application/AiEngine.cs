using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;

namespace AiAlgorithmsResearch.Core.AI.Application
{
    internal class AiEngine : IAiEngine
    {
        private readonly CombatActionCandidateProvider _actionProvider;

        public AiEngine(CombatActionCandidateProvider actionProvider)
        {
            _actionProvider = actionProvider;
        }

        public ICombatAction ProduceMove(ICombatAgent combatAgent, CombatAgentContext combatAgentContext)
        {
            var actionCandidates = _actionProvider.GetCandidates(combatAgentContext);
            return combatAgent.ChooseAction(actionCandidates);
        }
    }
}
