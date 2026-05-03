using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Api
{
    public interface ICombatAgent
    {
        ICombatAction ChooseAction(IList<ICombatAction> actions);
    }
}