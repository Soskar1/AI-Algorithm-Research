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

            TryHeal(simulation, executor, ref actionsToExecute);
            TryStun(simulation, executor, ref actionsToExecute);
            TryAttack(simulation, executor, ref actionsToExecute);
            TryTeleport(simulation, executor, ref actionsToExecute);
            TryMove(simulation, executor, ref actionsToExecute);

            return new CombatPlan(actionsToExecute);
        }

        private void TryHeal(SimulationCombatState simulation, EntityId executor, ref List<ICombatAction> actionsToExecute)
        {
            var energy = simulation.GetEnergy(executor);
            var health = simulation.GetHealth(executor);
            var maxHealth = simulation.GetMaxHealth(executor);

            var candidateActions = _combatActionCandidateProvider.GetCandidates(simulation, executor);

            if (energy > 0 && health / maxHealth <= _healingDecisionThreshold && TryGetAction(typeof(HealAction), candidateActions, out var healAction))
            {
                ExecuteAction(healAction, simulation, ref actionsToExecute, executor);
            }
        }

        private void TryStun(SimulationCombatState simulation, EntityId executor, ref List<ICombatAction> actionsToExecute)
        {
            var energy = simulation.GetEnergy(executor);
            var candidateActions = _combatActionCandidateProvider.GetCandidates(simulation, executor);

            if (energy > 0 && TryGetAction(typeof(StunAction), candidateActions, out var stunAction))
            {
                ExecuteAction(stunAction, simulation, ref actionsToExecute, executor);
            }
        }

        private void TryAttack(SimulationCombatState simulation, EntityId executor, ref List<ICombatAction> actionsToExecute)
        {
            var energy = simulation.GetEnergy(executor);
            var candidateActions = _combatActionCandidateProvider.GetCandidates(simulation, executor);

            while (energy > 0 && (TryGetAction(typeof(RangedAttackAction), candidateActions, out var attackAction) || TryGetAction(typeof(AttackAction), candidateActions, out attackAction)))
            {
                var executionSuccessfull = ExecuteAction(attackAction, simulation, ref actionsToExecute, executor);

                if (!executionSuccessfull)
                {
                    break;
                }
            }
        }

        private void TryTeleport(SimulationCombatState simulation, EntityId executor, ref List<ICombatAction> actionsToExecute)
        {
            var energy = simulation.GetEnergy(executor);
            var candidateActions = _combatActionCandidateProvider.GetCandidates(simulation, executor);

            if (energy > 0 && TryGetAction(typeof(TeleportAction), candidateActions, out var teleportAction))
            {
                ExecuteAction(teleportAction, simulation, ref actionsToExecute, executor);

                TryAttack(simulation, executor, ref actionsToExecute);
            }
        }

        private void TryMove(SimulationCombatState simulation, EntityId executor, ref List<ICombatAction> actionsToExecute)
        {
            var energy = simulation.GetEnergy(executor);
            var candidateActions = _combatActionCandidateProvider.GetCandidates(simulation, executor);

            if (energy > 0 && TryGetAction(typeof(MoveAction), candidateActions, out var moveAction))
            {
                ExecuteAction(moveAction, simulation, ref actionsToExecute, executor);

                TryAttack(simulation, executor, ref actionsToExecute);
            }
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

        private bool ExecuteAction(ICombatAction combatAction, SimulationCombatState simulation, ref List<ICombatAction> actionsToExecute, EntityId executor)
        {
            var executionSuccessfull = _combatActionExecutor.TryExecute(combatAction, simulation, simulation);

            if (executionSuccessfull)
            {
                actionsToExecute.Add(combatAction);
            }

            return executionSuccessfull;
        }
    }
}
