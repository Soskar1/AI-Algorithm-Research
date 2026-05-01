using AiAlgorithmsResearch.Core.Combat.Domain;
using AiAlgorithmsResearch.Core.Entities.Api;
using NUnit.Framework;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Tests
{
    public sealed class BattleTests
    {
        [Test]
        public void Current_WhenBattleCreated_ReturnsFirstParticipant()
        {
            var firstEntity = new TestEntity();
            var secondEntity = new TestEntity();

            var battle = new Battle(new List<BattleParticipant>
            {
                new BattleParticipant(firstEntity, initiative: 20),
                new BattleParticipant(secondEntity, initiative: 10)
            });

            Assert.AreSame(firstEntity, battle.Current.Entity);
        }

        [Test]
        public void NextTurn_WhenCalled_AdvancesCurrentParticipant()
        {
            var firstEntity = new TestEntity();
            var secondEntity = new TestEntity();

            var battle = new Battle(new List<BattleParticipant>
            {
                new BattleParticipant(firstEntity, initiative: 20),
                new BattleParticipant(secondEntity, initiative: 10)
            });

            battle.NextTurn();

            Assert.AreSame(secondEntity, battle.Current.Entity);
        }

        [Test]
        public void NextTurn_WhenCurrentIsLastParticipant_WrapsToFirstParticipant()
        {
            var firstEntity = new TestEntity();
            var secondEntity = new TestEntity();

            var battle = new Battle(new List<BattleParticipant>
            {
                new BattleParticipant(firstEntity, initiative: 20),
                new BattleParticipant(secondEntity, initiative: 10)
            });

            battle.NextTurn();
            battle.NextTurn();

            Assert.AreSame(firstEntity, battle.Current.Entity);
        }

        private sealed class TestEntity : IEntityView
        {
            public IHealthView Health => null;
            public IEnergyView Energy => null;
            public int Speed => 0;
            public int Strength => 0;
            public EntityId Id => throw new System.NotImplementedException();
        }
    }
}