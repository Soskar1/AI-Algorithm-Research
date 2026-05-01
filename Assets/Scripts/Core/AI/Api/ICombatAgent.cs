using AiAlgorithmsResearch.Core.Combat.Api;

namespace AiAlgorithmsResearch.Core.Ai.Api
{
    public interface ICombatAgent
    {
        ICombatAction ChooseAction(CombatAgentContext context);
    }
}