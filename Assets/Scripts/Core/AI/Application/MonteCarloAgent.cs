using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Ai.Domain;
using AiAlgorithmsResearch.Core.Combat.Api;
using System.Collections.Generic;
using UnityEngine;
using EntityId = AiAlgorithmsResearch.Core.Entities.Api.EntityId;
using Random = System.Random;

namespace AiAlgorithmsResearch.Core.Ai.Application
{
    internal class MonteCarloAgent : ICombatAgent
    {
        private readonly SimulationStateFactory _combatStateFactory;
        private readonly CombatActionCandidateProvider _combatActionCandidateProvider;
        private readonly ICombatActionExecutor _combatActionExecutor;
        private readonly Random _random;

        private readonly int _maxIterations;
        private readonly float _explorationRatio;

        public MonteCarloAgent(SimulationStateFactory combatStateFactory, CombatActionCandidateProvider combatActionCandidateProvider, ICombatActionExecutor combatActionExecutor, int maxIterations, Random random, float explorationRatio = 1.4142f)
        {
            _combatStateFactory = combatStateFactory;
            _combatActionCandidateProvider = combatActionCandidateProvider;
            _combatActionExecutor = combatActionExecutor;

            _maxIterations = maxIterations;
            _explorationRatio = explorationRatio;
            _random = random;
        }

        public CombatPlan ChoosePlan(ICombatStateView stateView, EntityId executor)
        {
            var simulation = _combatStateFactory.Create(stateView);
            var rootNode = new MonteCarloNode(simulation, _combatStateFactory, _combatActionCandidateProvider, _combatActionExecutor);
            var rootTeam = simulation.GetTeamId(executor);

            for (int i = 0; i < _maxIterations; ++i)
            {
                var node = Select(rootNode);

                if (!node.IsTerminal() && !node.IsFullyExpanded())
                {
                    node = node.Expand();
                }

                var winnerTeam = Rollout(node);

                node.Backpropagate(rootTeam, winnerTeam);
            }

            CombatPlan plan = null;
            if (rootNode.TryGetBestChild(out var bestNode))
            {
                plan = bestNode.CombatPlan;
            }

            return plan;
        }

        private MonteCarloNode Select(MonteCarloNode node)
        {
            while (node.IsFullyExpanded() && !node.IsTerminal() && node.HasChildren())
            {
                node = node.GetChildWithBestUct(_explorationRatio);
            }

            return node;
        }

        private TeamId Rollout(MonteCarloNode node)
        {
            var simulation = node.GetState();
            TeamId? winnerTeam;

            while (!simulation.IsTerminalState(out winnerTeam))
            {
                var actions = _combatActionCandidateProvider.GetCandidates(simulation, simulation.CurrentEntityTurn);
                var randomAction = actions[_random.Next(actions.Count)];

                Planning.SimulateTurn(randomAction, simulation, simulation.CurrentEntityTurn, _combatActionCandidateProvider, _combatActionExecutor, out _);

                simulation.NextTurn();
                simulation.TickCooldowns(simulation.CurrentEntityTurn);
                simulation.RegenerateEnergy(simulation.CurrentEntityTurn);
            }

            if (!winnerTeam.HasValue)
            {
                throw new System.Exception("oh no");
            }

            return winnerTeam.Value;
        }

        private class MonteCarloNode
        {
            private readonly SimulationCombatState _simulation;

            private readonly MonteCarloNode _parent;
            private readonly List<MonteCarloNode> _children;
            private readonly Stack<ICombatAction> _untriedActions;

            private readonly SimulationStateFactory _combatStateFactory;
            private readonly CombatActionCandidateProvider _combatActionCandidateProvider;
            private readonly ICombatActionExecutor _combatActionExecutor;

            public int NumberOfVisits { get; private set;  }
            public int NumberOfWins { get; private set; }

            public CombatPlan CombatPlan { get; private set; }

            public MonteCarloNode(SimulationCombatState simulation, SimulationStateFactory factory, CombatActionCandidateProvider candidateProvider, ICombatActionExecutor actionExecutor, MonteCarloNode parent = null, CombatPlan plan = null)
            {
                _simulation = simulation;
                _parent = parent;

                _children = new();
                NumberOfVisits = 0;
                NumberOfWins = 0;
                CombatPlan = plan;

                _combatStateFactory = factory;
                _combatActionCandidateProvider = candidateProvider;
                _combatActionExecutor = actionExecutor;

                var actionsToExplore = candidateProvider.GetCandidates(simulation, simulation.CurrentEntityTurn);
                _untriedActions = new Stack<ICombatAction>(actionsToExplore);
            }

            public MonteCarloNode Expand()
            {
                var isExecuted = false;
                CombatPlan plan = null;
                SimulationCombatState simulation = null;

                while (!isExecuted && _untriedActions.Count > 0)
                {
                    var action = _untriedActions.Pop();
                    simulation = _combatStateFactory.Create(_simulation);

                    isExecuted = Planning.SimulateTurn(action, simulation, simulation.CurrentEntityTurn, _combatActionCandidateProvider, _combatActionExecutor, out plan);
                }

                if (isExecuted)
                {
                    simulation.NextTurn();
                    simulation.TickCooldowns(simulation.CurrentEntityTurn);
                    simulation.RegenerateEnergy(simulation.CurrentEntityTurn);

                    var node = new MonteCarloNode(simulation, _combatStateFactory, _combatActionCandidateProvider, _combatActionExecutor, this, plan);
                    _children.Add(node);

                    return node;
                }

                return this;
            }

            public bool IsFullyExpanded() => _untriedActions.Count == 0;
            public bool HasChildren() => _children.Count > 0;
            public bool IsTerminal() => _simulation.IsTerminalState(out _);

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
                    var explorationComponent = explorationRatio * Mathf.Sqrt(Mathf.Log(NumberOfVisits) / node.NumberOfVisits);

                    var uct = exploitationComponent + explorationComponent;

                    if (uct > maxUct)
                    {
                        maxUct = uct;
                        bestUct = node;
                    }
                }

                return bestUct;
            }

            public SimulationCombatState GetState()
            {
                return _combatStateFactory.Create(_simulation);
            }

            public void Backpropagate(TeamId rootTeam, TeamId winner)
            {
                ++NumberOfVisits;

                if (winner == rootTeam)
                {
                    ++NumberOfWins;
                }

                _parent?.Backpropagate(rootTeam, winner);
            }
        }
    }
}
