using System;
using System.Collections.Generic;
using System.Linq;
using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Ai.Domain;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Maps.Api;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal sealed class CombatAgentFactory : ICombatAgentFactory
    {
        private readonly CombatActionCandidateProvider _provider;
        private readonly IReadOnlyTileMap _map;
        private readonly ICombatActionExecutor _executor;

        public CombatAgentFactory(CombatActionCandidateProvider provider, IReadOnlyTileMap map, ICombatActionExecutor executor)
        {
            _provider = provider;
            _map = map;
            _executor = executor;
        }

        public ICombatAgent CreateRandomAgent(Random random)
        {
            var simulationFactory = new SimulationStateFactory(_map);
            return new RandomCombatAgent(simulationFactory, _provider, _executor, random);
        }

        public ICombatAgent CreateStateMachineAgent()
        {
            var simulationFactory = new SimulationStateFactory(_map);
            return new StateMachineCombatAgent(simulationFactory, _provider, _executor);
        }

        public ICombatAgent CreateMinimaxAgent(int depth)
        {
            var simulationFactory = new SimulationStateFactory(_map);
            return new MinimaxCombatAgent(simulationFactory, _provider, _executor, depth, EvaluateState);
        }

        public ICombatAgent CreateDumbMinimaxAgent(int depth)
        {
            var simulationFactory = new SimulationStateFactory(_map);
            return new MinimaxCombatAgent(simulationFactory, _provider, _executor, depth, BadStateEvaluation);
        }

        public ICombatAgent CreateMonteCarloAgent(int maxIterations, Random random)
        {
            var simulationFactory = new SimulationStateFactory(_map);
            return new MonteCarloAgent(simulationFactory, _provider, _executor, maxIterations, random);
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
                + energyDifference;

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

        private float BadStateEvaluation(SimulationCombatState simulation, EntityId executor)
        {
            var executorTeam = simulation.GetEntityTeam(executor);
            var enemyTeam = simulation.EntityIds
                .Where(entity => !simulation.AreFriends(entity, executor))
                .ToList();

            (var executorHealthSum, var executorOverallHealth, var deadEntitiesExecutorTeam) = GetHealthStatistics(executorTeam);
            (var enemyHealthSum, var enemyOverallHealth, var deadEntitiesEnemyTeam) = GetHealthStatistics(enemyTeam);

            var executorEnergy = simulation.GetEnergy(executor);
            var executorMaxEnergy = simulation.GetMaxEnergy(executor);
            var energyDifference = executorMaxEnergy - executorEnergy;

            return executorHealthSum * 0.6f - enemyHealthSum * 0.9f + energyDifference;

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
        }
    }
}
