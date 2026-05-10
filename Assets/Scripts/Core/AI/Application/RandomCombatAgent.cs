using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;
using EntityId = AiAlgorithmsResearch.Core.Entities.Api.EntityId;
using Random = System.Random;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class RandomCombatAgent : ICombatAgent
    {
        private readonly SimulationStateFactory _combatStateFactory;
        private readonly CombatActionCandidateProvider _combatActionCandidateProvider;
        private readonly ICombatActionExecutor _combatActionExecutor;
        private readonly Random _random;

        public RandomCombatAgent(SimulationStateFactory combatStateFactory, CombatActionCandidateProvider combatActionCandidateProvider, ICombatActionExecutor combatActionExecutor, Random random)
        {
            _combatStateFactory = combatStateFactory;
            _combatActionCandidateProvider = combatActionCandidateProvider;
            _combatActionExecutor = combatActionExecutor;
            _random = random;
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

                if (actions == null || actions.Count == 0)
                {
                    break;
                }

                var randomAction = actions[_random.Next(actions.Count)];

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