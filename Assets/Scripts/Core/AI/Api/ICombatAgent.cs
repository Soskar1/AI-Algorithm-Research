using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Api
{
    public interface ICombatAgent
    {
        CombatPlan ChoosePlan(IList<ICombatAction> actions);
    }
}