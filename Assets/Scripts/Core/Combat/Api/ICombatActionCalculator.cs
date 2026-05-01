namespace AiAlgorithmsResearch.Core.Combat.Api
{
    public interface ICombatActionCostCalculator
    {
        int GetCost(ICombatAction action);
    }
}