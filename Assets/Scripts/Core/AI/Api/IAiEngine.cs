using AiAlgorithmsResearch.Core.Combat.Api;

namespace AiAlgorithmsResearch.Core.Ai.Api
{
    public interface IAiEngine
    {
        ICombatAction ProduceMove(ICombatAgent combatAgent, CombatAgentContext combatAgentContext);
    }
}
