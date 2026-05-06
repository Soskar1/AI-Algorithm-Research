using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class RandomCombatAgent : ICombatAgent
    {
        private readonly CombatStateFactory _combatStateFactory;

        public CombatPlan ChoosePlan(IList<ICombatAction> actions, CombatAgentContext context)
        {
            const int maxAttempts = 3;
            var currentAttempt = 0;
            var energyToSpend = context.Actor.Energy.Current;

            var actionsToExecute = new List<ICombatAction>();
            while (currentAttempt < maxAttempts && energyToSpend > 0)
            {
                var index = Random.Range(0, actions.Count);
                var action = actions[index];

                if (energyToSpend > action.Cost)
                {
                    actionsToExecute.Add(action);
                    energyToSpend -= action.Cost;
                }

                ++currentAttempt;
            }

            return new CombatPlan(actionsToExecute);
        }
    }
}