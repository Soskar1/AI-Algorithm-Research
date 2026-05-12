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

                var isExecuted = Planning.SimulateTurn(candidate, simulation, executor, _combatActionCandidateProvider, _combatActionExecutor, out var plan);
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
            if (currentDepth <= 0)
                return EvaluateState(simulation, executor);

            simulation.TickCooldowns(entity);
            simulation.RegenerateEnergy(entity);

            var turnBackup = _combatStateFactory.Create(simulation);
            var candidates = _combatActionCandidateProvider.GetCandidates(simulation, entity);

            if (simulation.AreFriends(entity, executor))
            {
                var maxEvaluation = float.MinValue;

                foreach (var candidate in candidates)
                {
                    var planBackup = _combatStateFactory.Create(simulation);

                    var isExecuted = Planning.SimulateTurn(candidate, simulation, entity, _combatActionCandidateProvider, _combatActionExecutor, out var plan);
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

                return maxEvaluation;
            }
            else
            {
                var minEvaluation = float.MaxValue;

                foreach (var candidate in candidates)
                {
                    var planBackup = _combatStateFactory.Create(simulation);

                    var isExecuted = Planning.SimulateTurn(candidate, simulation, entity, _combatActionCandidateProvider, _combatActionExecutor, out var plan);
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

                return minEvaluation;
            }
        }

        private float EvaluateState(SimulationCombatState simulation, EntityId executor)
        {
            var executorTeam = simulation.GetEntityTeam(executor);
            var enemyTeam = simulation.EntityIds
                .Where(entity => !simulation.AreFriends(entity, executor))
                .ToList();

            (var executorHealthSum, var executorOverallHealth, var deadEntitiesExecutorTeam) = GetHealthStatistics(executorTeam);
            (var enemyHealthSum, var enemyOverallHealth, var deadEntitiesEnemyTeam) = GetHealthStatistics(enemyTeam);

            var executorStunned = GetStunStatistics(executorTeam);
            var enemyStunned = GetStunStatistics(enemyTeam);

            var executorEnergy = simulation.GetEnergy(executor);
            var executorMaxEnergy = simulation.GetMaxEnergy(executor);
            var energyDifference = executorMaxEnergy - executorEnergy;

            return deadEntitiesEnemyTeam * 100 - deadEntitiesExecutorTeam * 100
                + executorHealthSum * 0.6f - enemyHealthSum * 0.9f
                + enemyStunned * 10 - executorStunned * 10
                + energyDifference * 0.1f;

            (int, int, int) GetHealthStatistics(IList<EntityId> entities)
            {
                var healthSum = 0;
                var deadEntities = 0;
                var overallHealth = 0;

                foreach (var entity in entities)
                {
                    var health = simulation.GetHealth(entity);
                    var maxHealth = simulation.GetMaxHealth(entity);

                    if (health <= 0)
                    {
                        ++deadEntities;
                    }

                    healthSum += health;
                    overallHealth += maxHealth;
                }

                return (healthSum, overallHealth, deadEntities);
            }

            int GetStunStatistics(IList<EntityId> entities)
            {
                var stunnedEntities = 0;

                foreach (var entity in entities)
                {
                    if (simulation.IsStunned(entity))
                    {
                        ++stunnedEntities;
                    }
                }

                return stunnedEntities;
            }
        }
    }
}
