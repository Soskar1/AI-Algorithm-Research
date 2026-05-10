using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Ai.Domain;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal class MinimaxCombatAgent : ICombatAgent
    {
        private readonly SimulationStateFactory _combatStateFactory;
        private readonly CombatActionCandidateProvider _combatActionCandidateProvider;
        private readonly ICombatActionExecutor _combatActionExecutor;
        private readonly int _depth;

        public MinimaxCombatAgent(SimulationStateFactory combatStateFactory, CombatActionCandidateProvider combatActionCandidateProvider, ICombatActionExecutor combatActionExecutor, int depth)
        {
            _combatStateFactory = combatStateFactory;
            _combatActionCandidateProvider = combatActionCandidateProvider;
            _combatActionExecutor = combatActionExecutor;
            _depth = depth;
        }

        public CombatPlan ChoosePlan(ICombatStateView stateView, EntityId executor)
        {
            var simulation = _combatStateFactory.Create(stateView);

            var alpha = float.MinValue;
            var beta = float.MaxValue;
            var bestValue = float.MinValue;

            CombatPlan bestPlan = null;

            var candidates = _combatActionCandidateProvider.GetCandidates(simulation, executor);

            foreach (var candidate in candidates)
            {
                var backup = _combatStateFactory.Create(simulation);

                var isExecuted = SimulateTurn(candidate, simulation, executor, out var plan);
                if (!isExecuted)
                {
                    simulation = backup;
                    continue;
                }

                simulation.NextTurn();
                var evaluation = Minimax(simulation, simulation.CurrentEntityTurn, executor, _depth - 1, alpha, beta);

                simulation = backup;

                if (evaluation > bestValue)
                {
                    bestValue = evaluation;
                    bestPlan = plan;
                }

                alpha = Math.Max(alpha, bestValue);
            }

            return bestPlan;
        }

        private float Minimax(SimulationCombatState simulation, EntityId entity, EntityId executor, int currentDepth, float alpha, float beta)
        {
            var health = simulation.GetHealth(entity);

            if (currentDepth <= 0 || health <= 0)
                return EvaluateState(simulation, executor);

            simulation.TickCooldowns(entity);
            simulation.RegenerateEnergy(entity);

            var turnBackup = _combatStateFactory.Create(simulation);
            var candidates = _combatActionCandidateProvider.GetCandidates(simulation, entity);

            if (!simulation.AreFriends(entity, executor))
            {
                float maxEvaluation = float.MinValue;
                foreach (var candidate in candidates)
                {
                    var planBackup = _combatStateFactory.Create(simulation);

                    var isExecuted = SimulateTurn(candidate, simulation, entity, out var plan);
                    if (!isExecuted)
                    {
                        simulation = planBackup;
                        continue;
                    }

                    simulation.NextTurn();
                    var evaluation = Minimax(simulation, simulation.CurrentEntityTurn, executor, currentDepth - 1, alpha, beta);

                    simulation = planBackup;

                    maxEvaluation = Math.Max(maxEvaluation, evaluation);
                    alpha = Math.Max(alpha, evaluation);

                    if (beta <= alpha)
                        break;
                }

                simulation = turnBackup;
                return maxEvaluation;
            }
            else
            {
                var minEvaluation = float.MaxValue;
                foreach (var candidate in candidates)
                {
                    var planBackup = _combatStateFactory.Create(simulation);

                    var isExecuted = SimulateTurn(candidate, simulation, entity, out var plan);
                    if (!isExecuted)
                    {
                        simulation = planBackup;
                        continue;
                    }

                    simulation.NextTurn();
                    var evaluation = Minimax(simulation, simulation.CurrentEntityTurn, executor, currentDepth - 1, alpha, beta);

                    simulation = planBackup;

                    minEvaluation = Math.Min(minEvaluation, evaluation);
                    beta = Math.Min(beta, evaluation);

                    if (beta <= alpha)
                        break;
                }

                simulation = turnBackup;
                return minEvaluation;
            }
        }

        private float EvaluateState(SimulationCombatState simulation, EntityId executor)
        {
            var executorTeam = simulation.GetEntityTeam(executor);
            var enemyTeam = simulation.EntityIds
                .Where(entity => !simulation.AreFriends(entity, executor))
                .ToList();

            (var healthCoefficientExecutorTeam, var deadEntitiesExecutorTeam) = GetHealthStatistics(executorTeam);
            (var healthCoefficientEnemyTeam, var deadEntitiesEnemyTeam) = GetHealthStatistics(executorTeam);

            return deadEntitiesEnemyTeam * 100 - deadEntitiesExecutorTeam * 100
                + healthCoefficientExecutorTeam - healthCoefficientEnemyTeam;

            (float, int) GetHealthStatistics(IList<EntityId> entities)
            {
                var healthCoefficient = 0;
                var deadEntities = 0;

                foreach (var entity in entities)
                {
                    var health = simulation.GetHealth(entity);
                    var maxHealth = simulation.GetMaxHealth(entity);

                    if (health <= 0)
                    {
                        ++deadEntities;
                    }
                    else
                    {
                        healthCoefficient += health / maxHealth;
                    }
                }

                return (healthCoefficient, deadEntities);
            }
        }

        private bool SimulateTurn(ICombatAction mainAction, SimulationCombatState simulation, EntityId executor, out CombatPlan plan)
        {
            var actionsToExecute = new List<ICombatAction>();
            var energy = simulation.GetEnergy(executor);
            var candidates = _combatActionCandidateProvider.GetCandidates(simulation, executor);
            var executionSuccessfull = ExecuteAction(mainAction, simulation, actionsToExecute, candidates, executor, ref energy);
            plan = null;

            if (!executionSuccessfull)
            {
                return false;
            }
            
            var actionType = mainAction.GetType();
            if (actionType == typeof(HealAction))
            {
                SimulateHealTurnPlan(simulation, executor, actionsToExecute, ref energy);
            }
            else if (actionType == typeof(StunAction))
            {
                SimulateStunTurnPlan(simulation, executor, actionsToExecute, ref energy);
            }
            else if (actionType == typeof(AttackAction))
            {
                SimulateAttackTurnPlan(simulation, executor, actionsToExecute, ref energy);
            }
            else if (actionType == typeof(MoveAction) || actionType == typeof(TeleportAction))
            {
                SimulateAttackTurnPlan(simulation, executor, actionsToExecute, ref energy);
            }

            plan = new CombatPlan(actionsToExecute);
            return true;
        }

        private void SimulateHealTurnPlan(SimulationCombatState simulation, EntityId executor, List<ICombatAction> actionsToExecute, ref int energy)
        {
            var candidates = _combatActionCandidateProvider.GetCandidates(simulation, executor);

            if (energy > 0 && TryGetAction(typeof(StunAction), candidates, out var stunAction))
            {
                ExecuteAction(stunAction, simulation, actionsToExecute, candidates, executor, ref energy);
            }

            while (energy > 0 && TryGetAction(typeof(AttackAction), candidates, out var attackAction))
            {
                var executionSuccessfull = ExecuteAction(attackAction, simulation, actionsToExecute, candidates, executor, ref energy);

                if (!executionSuccessfull)
                {
                    break;
                }
            }

            if (energy > 0 && TryGetAction(typeof(TeleportAction), candidates, out var teleportAction))
            {
                ExecuteAction(teleportAction, simulation, actionsToExecute, candidates, executor, ref energy);

                while (energy > 0 && TryGetAction(typeof(AttackAction), candidates, out var attackAction))
                {
                    var executionSuccessfull = ExecuteAction(attackAction, simulation, actionsToExecute, candidates, executor, ref energy);

                    if (!executionSuccessfull)
                    {
                        break;
                    }
                }
            }

            if (energy > 0 && TryGetAction(typeof(MoveAction), candidates, out var moveAction))
            {
                ExecuteAction(moveAction, simulation, actionsToExecute, candidates, executor, ref energy);

                while (energy > 0 && TryGetAction(typeof(AttackAction), candidates, out var attackAction))
                {
                    var executionSuccessfull = ExecuteAction(attackAction, simulation, actionsToExecute, candidates, executor, ref energy);

                    if (!executionSuccessfull)
                    {
                        break;
                    }
                }
            }
        }

        private void SimulateStunTurnPlan(SimulationCombatState simulation, EntityId executor, List<ICombatAction> actionsToExecute, ref int energy)
        {
            var candidates = _combatActionCandidateProvider.GetCandidates(simulation, executor);

            while (energy > 0 && TryGetAction(typeof(AttackAction), candidates, out var attackAction))
            {
                var executionSuccessfull = ExecuteAction(attackAction, simulation, actionsToExecute, candidates, executor, ref energy);

                if (!executionSuccessfull)
                {
                    break;
                }
            }
        }

        private void SimulateAttackTurnPlan(SimulationCombatState simulation, EntityId executor, List<ICombatAction> actionsToExecute, ref int energy)
        {
            var candidates = _combatActionCandidateProvider.GetCandidates(simulation, executor);

            while (energy > 0 && TryGetAction(typeof(AttackAction), candidates, out var attackAction))
            {
                var executionSuccessfull = ExecuteAction(attackAction, simulation, actionsToExecute, candidates, executor, ref energy);

                if (!executionSuccessfull)
                {
                    break;
                }
            }
        }

        private void SimulateApproachingTurnPlan(SimulationCombatState simulation, EntityId executor, List<ICombatAction> actionsToExecute, ref int energy)
        {
            var candidates = _combatActionCandidateProvider.GetCandidates(simulation, executor);

            if (energy > 0 && TryGetAction(typeof(TeleportAction), candidates, out var teleportAction))
            {
                ExecuteAction(teleportAction, simulation, actionsToExecute, candidates, executor, ref energy);

                while (energy > 0 && TryGetAction(typeof(AttackAction), candidates, out var attackAction))
                {
                    var executionSuccessfull = ExecuteAction(attackAction, simulation, actionsToExecute, candidates, executor, ref energy);

                    if (!executionSuccessfull)
                    {
                        break;
                    }
                }
            }

            if (energy > 0 && TryGetAction(typeof(MoveAction), candidates, out var moveAction))
            {
                ExecuteAction(moveAction, simulation, actionsToExecute, candidates, executor, ref energy);

                while (energy > 0 && TryGetAction(typeof(AttackAction), candidates, out var attackAction))
                {
                    var executionSuccessfull = ExecuteAction(attackAction, simulation, actionsToExecute, candidates, executor, ref energy);

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

        private bool ExecuteAction(ICombatAction combatAction, SimulationCombatState simulation, List<ICombatAction> actionsToExecute, IList<ICombatAction> candidateActions, EntityId executor, ref int energy)
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
