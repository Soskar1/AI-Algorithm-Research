using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Ai.Domain;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using System;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal class MinimaxCombatAgent : ICombatAgent
    {
        private readonly SimulationStateFactory _combatStateFactory;
        private readonly CombatActionCandidateProvider _combatActionCandidateProvider;
        private readonly ICombatActionExecutor _combatActionExecutor;
        private readonly int _depth;
        private readonly Func<SimulationCombatState, EntityId, float> _stateEvaluation;

        public MinimaxCombatAgent(SimulationStateFactory combatStateFactory, CombatActionCandidateProvider combatActionCandidateProvider, ICombatActionExecutor combatActionExecutor, int depth, Func<SimulationCombatState, EntityId, float> stateEvaluation)
        {
            _combatStateFactory = combatStateFactory;
            _combatActionCandidateProvider = combatActionCandidateProvider;
            _combatActionExecutor = combatActionExecutor;
            _depth = depth;
            _stateEvaluation = stateEvaluation;
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
                return _stateEvaluation(simulation, executor);

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
    }
}
