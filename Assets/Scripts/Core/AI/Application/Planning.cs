using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Ai.Domain;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal static class Planning
    {
        public static bool SimulateTurn(ICombatAction mainAction, SimulationCombatState simulation, EntityId executor, CombatActionCandidateProvider candidateProvider, ICombatActionExecutor actionExecutor, out CombatPlan plan)
        {
            if (simulation.IsStunned(executor) || simulation.GetHealth(executor) <= 0)
            {
                plan = new CombatPlan(new List<ICombatAction>() { new WaitAction(executor) });
                return true;
            }

            var actionsToExecute = new List<ICombatAction>();
            var energy = simulation.GetEnergy(executor);
            var candidates = candidateProvider.GetCandidates(simulation, executor);
            var executionSuccessfull = ExecuteAction(mainAction, simulation, ref actionsToExecute, ref candidates, executor, actionExecutor, candidateProvider, ref energy);
            plan = null;

            if (!executionSuccessfull)
            {
                return false;
            }

            var actionType = mainAction.GetType();
            if (actionType == typeof(HealAction))
            {
                SimulateHealTurnPlan(simulation, executor, actionsToExecute, candidateProvider, actionExecutor, ref energy);
            }
            else if (actionType == typeof(StunAction) || actionType == typeof(RangedAttackAction) || actionType == typeof(AttackAction) || actionType == typeof(MoveAction) || actionType == typeof(TeleportAction))
            {
                SimulateAttackTurnPlan(simulation, executor, actionsToExecute, candidateProvider, actionExecutor, ref energy);
            }

            plan = new CombatPlan(actionsToExecute);
            return true;
        }

        private static void SimulateHealTurnPlan(SimulationCombatState simulation, EntityId executor, List<ICombatAction> actionsToExecute, CombatActionCandidateProvider candidateProvider, ICombatActionExecutor actionExecutor, ref int energy)
        {
            var candidates = candidateProvider.GetCandidates(simulation, executor);

            if (energy > 0 && TryGetAction(typeof(StunAction), candidates, out var stunAction))
            {
                ExecuteAction(stunAction, simulation, ref actionsToExecute, ref candidates, executor, actionExecutor, candidateProvider, ref energy);
            }

            SimulateAttackTurnPlan(simulation, executor, actionsToExecute, candidateProvider, actionExecutor, ref energy);

            if (energy > 0 && TryGetAction(typeof(TeleportAction), candidates, out var teleportAction))
            {
                ExecuteAction(teleportAction, simulation, ref actionsToExecute, ref candidates, executor, actionExecutor, candidateProvider, ref energy);

                SimulateAttackTurnPlan(simulation, executor, actionsToExecute, candidateProvider, actionExecutor, ref energy);
            }

            if (energy > 0 && TryGetAction(typeof(MoveAction), candidates, out var moveAction))
            {
                ExecuteAction(moveAction, simulation, ref actionsToExecute, ref candidates, executor, actionExecutor, candidateProvider, ref energy);

                SimulateAttackTurnPlan(simulation, executor, actionsToExecute, candidateProvider, actionExecutor, ref energy);
            }
        }

        private static void SimulateAttackTurnPlan(SimulationCombatState simulation, EntityId executor, List<ICombatAction> actionsToExecute, CombatActionCandidateProvider candidateProvider, ICombatActionExecutor actionExecutor, ref int energy)
        {
            var candidates = candidateProvider.GetCandidates(simulation, executor);

            while (energy > 0 && (TryGetAction(typeof(RangedAttackAction), candidates, out var attackAction) || TryGetAction(typeof(AttackAction), candidates, out attackAction)))
            {
                var executionSuccessfull = ExecuteAction(attackAction, simulation, ref actionsToExecute, ref candidates, executor, actionExecutor, candidateProvider, ref energy);

                if (!executionSuccessfull)
                {
                    break;
                }
            }

            while (energy > 0 && TryGetAction(typeof(MoveAction), candidates, out var moveAction))
            {
                ExecuteAction(moveAction, simulation, ref actionsToExecute, ref candidates, executor, actionExecutor, candidateProvider, ref energy);

                while (energy > 0 && (TryGetAction(typeof(RangedAttackAction), candidates, out var attackAction) || TryGetAction(typeof(AttackAction), candidates, out attackAction)))
                {
                    var executionSuccessfull = ExecuteAction(attackAction, simulation, ref actionsToExecute, ref candidates, executor, actionExecutor, candidateProvider, ref energy);

                    if (!executionSuccessfull)
                    {
                        break;
                    }
                }
            }
        }

        private static bool TryGetAction(Type actionType, IEnumerable<ICombatAction> actions, out ICombatAction foundAction)
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

        private static bool ExecuteAction(ICombatAction combatAction, SimulationCombatState simulation, ref List<ICombatAction> actionsToExecute, ref IList<ICombatAction> candidateActions, EntityId executor, ICombatActionExecutor combatActionExecutor, CombatActionCandidateProvider candidateProvider, ref int energy)
        {
            var executionSuccessfull = combatActionExecutor.TryExecute(combatAction, simulation, simulation);

            if (executionSuccessfull)
            {
                actionsToExecute.Add(combatAction);
                energy = simulation.GetEnergy(executor);

                if (energy > 0)
                {
                    candidateActions = candidateProvider.GetCandidates(simulation, executor);
                }
            }

            return executionSuccessfull;
        }
    }
}
