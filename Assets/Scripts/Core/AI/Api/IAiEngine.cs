namespace AiAlgorithmsResearch.Core.Ai.Api
{
    public interface IAiEngine
    {
        CombatPlan ProduceMove(ICombatAgent combatAgent, CombatAgentContext combatAgentContext);
    }
}
