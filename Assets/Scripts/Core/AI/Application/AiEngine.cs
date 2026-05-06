using AiAlgorithmsResearch.Core.Ai.Api;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal class AiEngine : IAiEngine
    {
        private readonly CombatActionCandidateProvider _actionProvider;

        public AiEngine(CombatActionCandidateProvider actionProvider)
        {
            _actionProvider = actionProvider;
        }

        public CombatPlan ProduceMove(ICombatAgent combatAgent, CombatAgentContext combatAgentContext)
        {
            var actionCandidates = _actionProvider.GetCandidates(combatAgentContext);
            return combatAgent.ChoosePlan(actionCandidates, combatAgentContext);
        }
    }
}
