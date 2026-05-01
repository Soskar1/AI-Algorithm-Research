using System.Collections.Generic;
using AiAlgorithmsResearch.Core.Ai.Api;
using AiAlgorithmsResearch.Core.Ai.Application;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Entities.Api;
using NUnit.Framework;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Ai.Tests
{
    public sealed class RandomCombatAgentTests
    {
        [Test]
        public void ChooseAction_WhenAvailableActionsExist_ReturnsActionAtRandomIndex()
        {
            var actor = new TestEntity();
            var random = new FakeRandomNumberGenerator(indexToReturn: 1);

            var agent = new RandomCombatAgent(random);

            var firstAction = new WaitAction(actor);
            var secondAction = new MoveAction(actor, new Vector2Int(1, 1));

            var context = new CombatAgentContext(actor,
                new ICombatAction[]
                {
                    firstAction,
                    secondAction
                });

            var chosenAction = agent.ChooseAction(context);

            Assert.AreEqual(secondAction, chosenAction);
        }

        [Test]
        public void ChooseAction_WhenNoActionsExist_ReturnsWaitAction()
        {
            var actor = new TestEntity();
            var random = new FakeRandomNumberGenerator(indexToReturn: 0);

            var agent = new RandomCombatAgent(random);

            var context = new CombatAgentContext(actor, new List<ICombatAction>());

            var chosenAction = agent.ChooseAction(context);

            Assert.IsInstanceOf<WaitAction>(chosenAction);
            Assert.AreSame(actor, chosenAction.Actor);
        }

        private sealed class FakeRandomNumberGenerator : IRandomNumberGenerator
        {
            private readonly int _indexToReturn;

            public FakeRandomNumberGenerator(int indexToReturn)
            {
                _indexToReturn = indexToReturn;
            }

            public int Range(int minInclusive, int maxExclusive)
            {
                return _indexToReturn;
            }
        }

        private sealed class TestEntity : IEntityView
        {
            public Entities.Api.EntityId Id { get; } = new Entities.Api.EntityId(System.Guid.NewGuid());
            public IHealthView Health => null;
            public IEnergyView Energy => null;
            public int Speed => 0;
            public int Strength => 0;
        }
    }
}