using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class RandomCombatAgent : ICombatAgent
    {
        private readonly SimulationStateFactory _combatStateFactory;
        private readonly CombatActionCandidateProvider _combatActionCandidateProvider;

        public RandomCombatAgent(SimulationStateFactory combatStateFactory, CombatActionCandidateProvider combatActionCandidateProvider)
        {
            _combatStateFactory = combatStateFactory;
            _combatActionCandidateProvider = combatActionCandidateProvider;
        }

        public CombatPlan ChoosePlan(ICombatStateView stateView, EntityId executor)
        {
            var simulation = _combatStateFactory.Create(stateView);
            var energy = simulation.GetEnergy(executor);

            var actions = _combatActionCandidateProvider.GetCandidates(simulation, executor);

            return CombatPlan.Single(actions[0]);

            // TODO

            //var actionsToExecute = new List<ICombatAction>();
            //return new CombatPlan(actionsToExecute);
        }
    }
}