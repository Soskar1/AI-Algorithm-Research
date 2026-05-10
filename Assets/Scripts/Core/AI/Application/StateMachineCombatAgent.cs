using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Ai.Domain;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal class StateMachineCombatAgent : ICombatAgent
    {
        private readonly SimulationStateFactory _combatStateFactory;
        private readonly CombatActionCandidateProvider _combatActionCandidateProvider;
        private readonly ICombatActionExecutor _combatActionExecutor;

        private const float _healingDecisionThreshold = 0.25f;

        public StateMachineCombatAgent(SimulationStateFactory combatStateFactory, CombatActionCandidateProvider combatActionCandidateProvider, ICombatActionExecutor combatActionExecutor)
        {
            _combatStateFactory = combatStateFactory;
            _combatActionCandidateProvider = combatActionCandidateProvider;
            _combatActionExecutor = combatActionExecutor;
        }

        public CombatPlan ChoosePlan(ICombatStateView stateView, EntityId executor)
        {
            var simulation = _combatStateFactory.Create(stateView);
            var actionsToExecute = new List<ICombatAction>();

            var energy = simulation.GetEnergy(executor);
            var health = simulation.GetHealth(executor);
            var maxHealth = simulation.GetMaxHealth(executor);

            var candidateActions = _combatActionCandidateProvider.GetCandidates(simulation, executor);

            if (energy > 0 && health / maxHealth <= _healingDecisionThreshold && TryGetAction(typeof(HealAction), candidateActions, out var healAction))
            {
                ExecuteAction(healAction, simulation, ref actionsToExecute, ref candidateActions, executor, ref energy);
            }

            if (energy > 0 && TryGetAction(typeof(StunAction), candidateActions, out var stunAction))
            {
                ExecuteAction(stunAction, simulation, ref actionsToExecute, ref candidateActions, executor, ref energy);
            }

            while (energy > 0 && TryGetAction(typeof(AttackAction), candidateActions, out var attackAction))
            {
                var executionSuccessfull = ExecuteAction(attackAction, simulation, ref actionsToExecute, ref candidateActions, executor, ref energy);

                if (!executionSuccessfull)
                {
                    break;
                }
            }

            if (energy > 0 && TryGetAction(typeof(TeleportAction), candidateActions, out var teleportAction))
            {
                ExecuteAction(teleportAction, simulation, ref actionsToExecute, ref candidateActions, executor, ref energy);

                while (energy > 0 && TryGetAction(typeof(AttackAction), candidateActions, out var attackAction))
                {
                    var executionSuccessfull = ExecuteAction(attackAction, simulation, ref actionsToExecute, ref candidateActions, executor, ref energy);

                    if (!executionSuccessfull)
                    {
                        break;
                    }
                }
            }

            if (energy > 0 && TryGetAction(typeof(MoveAction), candidateActions, out var moveAction))
            {
                ExecuteAction(moveAction, simulation, ref actionsToExecute, ref candidateActions, executor, ref energy);

                while (energy > 0 && TryGetAction(typeof(AttackAction), candidateActions, out var attackAction))
                {
                    var executionSuccessfull = ExecuteAction(attackAction, simulation, ref actionsToExecute, ref candidateActions, executor, ref energy);

                    if (!executionSuccessfull)
                    {
                        break;
                    }
                }
            }

            return new CombatPlan(actionsToExecute);
        }

        private bool TryGetAction(Type actionType, IEnumerable<ICombatAction> actions, out ICombatAction foundAction)
        {
            foundAction = null;

            foreach (var action in actions)
            {
                if (action.GetType() == actionType)
                {
                    foundAction = action;
                    return true;
                }
            }

            return false;
        }

        private bool ExecuteAction(ICombatAction combatAction, SimulationCombatState simulation, ref List<ICombatAction> actionsToExecute, ref IList<ICombatAction> candidateActions, EntityId executor, ref int energy)
        {
            var executionSuccessfull = _combatActionExecutor.TryExecute(combatAction, simulation, simulation);

            if (executionSuccessfull)
            {
                actionsToExecute.Add(combatAction);
                energy = simulation.GetEnergy(executor);

                if (energy > 0)
                {
                    candidateActions = _combatActionCandidateProvider.GetCandidates(simulation, executor);
                }
            }

            return executionSuccessfull;
        }
    }
}
