using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;

namespace AiAlgorithmsResearch.Core.Ai.Api
{
    public interface ICombatAgent
    {
        CombatPlan ChoosePlan(ICombatStateView stateView, EntityId executor);
    }
}