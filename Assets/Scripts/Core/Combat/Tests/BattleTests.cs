using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Combat.Domain;
using AiAlgorithmsResearch.Core.Entities.Api;
using NUnit.Framework;
using System.Collections.Generic;

namespace AiAlgorithmsResearch.Core.Combat.Tests
{
    public sealed class BattleTests
    {
        private static readonly TeamId TeamA = new(1);
        private static readonly TeamId TeamB = new(2);

        [Test]
        public void Current_WhenBattleCreated_ReturnsFirstParticipant()
        {
            var firstEntity = new TestEntity();
            var secondEntity = new TestEntity();

            var battle = new Battle(new List<BattleParticipant>
            {
                new BattleParticipant(firstEntity, initiative: 20, TeamA),
                new BattleParticipant(secondEntity, initiative: 10, TeamB)
            });

            Assert.AreSame(firstEntity, battle.Current.Entity);
            Assert.AreEqual(TeamA, battle.Current.TeamId);
        }

        [Test]
        public void NextTurn_WhenCalled_AdvancesCurrentParticipant()
        {
            var firstEntity = new TestEntity();
            var secondEntity = new TestEntity();

            var battle = new Battle(new List<BattleParticipant>
            {
                new BattleParticipant(firstEntity, initiative: 20, TeamA),
                new BattleParticipant(secondEntity, initiative: 10, TeamB)
            });

            battle.NextTurn();

            Assert.AreSame(secondEntity, battle.Current.Entity);
            Assert.AreEqual(TeamB, battle.Current.TeamId);
        }

        [Test]
        public void NextTurn_WhenCurrentIsLastParticipant_WrapsToFirstParticipant()
        {
            var firstEntity = new TestEntity();
            var secondEntity = new TestEntity();

            var battle = new Battle(new List<BattleParticipant>
            {
                new BattleParticipant(firstEntity, initiative: 20, TeamA),
                new BattleParticipant(secondEntity, initiative: 10, TeamB)
            });

            battle.NextTurn();
            battle.NextTurn();

            Assert.AreSame(firstEntity, battle.Current.Entity);
            Assert.AreEqual(TeamA, battle.Current.TeamId);
        }

        private sealed class TestEntity : IEntityView
        {
            public EntityId Id { get; } = new EntityId(System.Guid.NewGuid());
            public IHealthView Health => null;
            public IEnergyView Energy => null;
            public int Speed => 0;
            public int Strength => 0;
        }
    }
}