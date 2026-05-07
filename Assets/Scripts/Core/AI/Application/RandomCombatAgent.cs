using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;
using UnityEngine;
using EntityId = AiAlgorithmsResearch.Core.Entities.Api.EntityId;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class RandomCombatAgent : ICombatAgent
    {
        private readonly SimulationStateFactory _combatStateFactory;
        private readonly CombatActionCandidateProvider _combatActionCandidateProvider;
        private readonly ICombatActionExecutor _combatActionExecutor;

        public RandomCombatAgent(SimulationStateFactory combatStateFactory, CombatActionCandidateProvider combatActionCandidateProvider, ICombatActionExecutor combatActionExecutor)
        {
            _combatStateFactory = combatStateFactory;
            _combatActionCandidateProvider = combatActionCandidateProvider;
            _combatActionExecutor = combatActionExecutor;
        }

        public CombatPlan ChoosePlan(ICombatStateView stateView, EntityId executor)
        {
            const int maxAttempts = 3;
            var currentAttempt = 0;

            var simulation = _combatStateFactory.Create(stateView);
            var energy = simulation.GetEnergy(executor);
            var actionsToExecute = new List<ICombatAction>();
            
            while (energy > 0 && currentAttempt < maxAttempts)
            {
                var actions = _combatActionCandidateProvider.GetCandidates(simulation, executor);
                var randomAction = actions[Random.Range(0, actions.Count)];

                var executionSuccessfull = _combatActionExecutor.TryExecute(randomAction, simulation, simulation);
                if (executionSuccessfull)
                {
                    actionsToExecute.Add(randomAction);
                }

                energy = simulation.GetEnergy(executor);
                ++currentAttempt;
            }

            return new CombatPlan(actionsToExecute);
        }
    }
}