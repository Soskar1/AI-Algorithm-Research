using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Ai.Domain;
using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;
using UnityEngine;
using EntityId = AiAlgorithmsResearch.Core.Entities.Api.EntityId;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal class MonteCarloAgent : ICombatAgent
    {
        private readonly SimulationStateFactory _combatStateFactory;
        private readonly CombatActionCandidateProvider _combatActionCandidateProvider;
        private readonly ICombatActionExecutor _combatActionExecutor;

        private readonly int _maxIterations;
        private readonly float _explorationRatio;

        public MonteCarloAgent(SimulationStateFactory combatStateFactory, CombatActionCandidateProvider combatActionCandidateProvider, ICombatActionExecutor combatActionExecutor, int maxIterations, float explorationRatio = 1.4142f)
        {
            _combatStateFactory = combatStateFactory;
            _combatActionCandidateProvider = combatActionCandidateProvider;
            _combatActionExecutor = combatActionExecutor;

            _maxIterations = maxIterations;
            _explorationRatio = explorationRatio;
        }

        public CombatPlan ChoosePlan(ICombatStateView stateView, EntityId executor)
        {
            // Selection
            // Expansion
            // Simulation
            // Backpropagation

            var simulation = _combatStateFactory.Create(stateView);
            var rootNode = new MonteCarloNode(simulation, _combatStateFactory, _combatActionCandidateProvider, _combatActionExecutor);

            while (!rootNode.IsFullyExpanded())
            {
                rootNode.Expand();
            }

            for (int i = 0; i < _maxIterations; ++i)
            {

            }

            return null;
        }



        private class MonteCarloNode
        {
            private readonly SimulationCombatState _simulation;
            private readonly CombatPlan _plan;

            private readonly MonteCarloNode _parent;
            private readonly List<MonteCarloNode> _children;
            private readonly Stack<ICombatAction> _untriedActions;

            private readonly SimulationStateFactory _combatStateFactory;
            private readonly CombatActionCandidateProvider _combatActionCandidateProvider;
            private readonly ICombatActionExecutor _combatActionExecutor;

            private int _numberOfVisits;
            private int _numberOfWins;

            public int NumberOfVisits => _numberOfVisits;
            public int NumberOfWins => _numberOfWins;

            public MonteCarloNode(SimulationCombatState simulation, SimulationStateFactory factory, CombatActionCandidateProvider candidateProvider, ICombatActionExecutor actionExecutor, MonteCarloNode parent = null, CombatPlan plan = null)
            {
                _simulation = simulation;
                _parent = parent;

                _children = new();
                _numberOfVisits = 0;
                _numberOfWins = 0;
                _plan = plan;

                var actionsToExplore = candidateProvider.GetCandidates(simulation, simulation.CurrentEntityTurn);
                _untriedActions = new Stack<ICombatAction>(actionsToExplore);
            }

            public void Expand()
            {
                var action = _untriedActions.Pop();

                var simulation = _combatStateFactory.Create(_simulation);

                simulation.NextTurn();
                var executor = simulation.CurrentEntityTurn;

                simulation.TickCooldowns(executor);
                simulation.RegenerateEnergy(executor);

                var isExecuted = Planning.SimulateTurn(action, simulation, executor, _combatActionCandidateProvider, _combatActionExecutor, out CombatPlan plan);
                if (isExecuted)
                {
                    var node = new MonteCarloNode(simulation, _combatStateFactory, _combatActionCandidateProvider, _combatActionExecutor, this, plan);
                    _children.Add(node);
                }
            }

            public bool IsFullyExpanded() => _untriedActions.Count == 0;

            public bool TryGetBestChild(out MonteCarloNode bestNode)
            {
                var maxValue = int.MinValue;
                bestNode = null;

                foreach (var node in _children)
                {
                    if (node.NumberOfVisits > maxValue)
                    {
                        maxValue = node.NumberOfVisits;
                        bestNode = node;
                    }
                }

                if (bestNode == null)
                {
                    return false;
                }

                return true;
            }

            public MonteCarloNode GetChildWithBestUct(float explorationRatio)
            {
                var maxUct = float.MinValue;
                MonteCarloNode bestUct = null;

                foreach (var node in _children)
                {
                    if (node.NumberOfVisits == 0)
                    {
                        return node;
                    }

                    var exploitationComponent = node.NumberOfWins / (float)node.NumberOfVisits;
                    var explorationComponent = explorationRatio * Mathf.Sqrt(Mathf.Log(_numberOfWins) / node.NumberOfVisits);

                    var uct = exploitationComponent + explorationComponent;

                    if (uct > maxUct)
                    {
                        maxUct = uct;
                        bestUct = node;
                    }
                }

                return bestUct;
            }
        }
    }
}
