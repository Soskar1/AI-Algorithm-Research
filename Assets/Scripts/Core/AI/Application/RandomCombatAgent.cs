using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class RandomCombatAgent : ICombatAgent
    {
        public CombatPlan ChoosePlan(IList<ICombatAction> actions)
        {
            var index = Random.Range(0, actions.Count);

            return CombatPlan.Single(actions[index]);
        }
    }
}