using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Combat.Domain;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Entities.Application;
using NUnit.Framework;

namespace AiAlgorithmsResearch.Core.Combat.Tests
{
    public sealed class StunStatusTests
    {
        private IEntityFactory _entityFactory;
        private IStunStatus _stunStatus;
        private IStunStatusEditor _stunStatusEditor;

        [SetUp]
        public void SetUp()
        {
            _entityFactory = new EntityFactory();

            var stunStatus = new StunStatus();
            _stunStatus = stunStatus;
            _stunStatusEditor = stunStatus;
        }

        [Test]
        public void IsStunned_WhenEntityWasNotStunned_ReturnsFalse()
        {
            var entity = CreateEntity();

            Assert.IsFalse(_stunStatus.IsStunned(entity));
        }

        [Test]
        public void StunForNextTurn_WhenCalled_MarksEntityAsStunned()
        {
            var entity = CreateEntity();

            _stunStatusEditor.StunForNextTurn(entity);

            Assert.IsTrue(_stunStatus.IsStunned(entity));
        }

        [Test]
        public void ConsumeStun_WhenEntityIsStunned_ReturnsTrueAndRemovesStun()
        {
            var entity = CreateEntity();

            _stunStatusEditor.StunForNextTurn(entity);

            var result = _stunStatusEditor.ConsumeStun(entity);

            Assert.IsTrue(result);
            Assert.IsFalse(_stunStatus.IsStunned(entity));
        }

        [Test]
        public void ConsumeStun_WhenEntityIsNotStunned_ReturnsFalse()
        {
            var entity = CreateEntity();

            var result = _stunStatusEditor.ConsumeStun(entity);

            Assert.IsFalse(result);
        }

        [Test]
        public void StunForNextTurn_WhenOtherEntityIsStunned_DoesNotStunThisEntity()
        {
            var stunnedEntity = CreateEntity();
            var otherEntity = CreateEntity();

            _stunStatusEditor.StunForNextTurn(stunnedEntity);

            Assert.IsTrue(_stunStatus.IsStunned(stunnedEntity));
            Assert.IsFalse(_stunStatus.IsStunned(otherEntity));
        }

        [Test]
        public void StunForNextTurn_WhenCalledMultipleTimes_StillConsumesOnce()
        {
            var entity = CreateEntity();

            _stunStatusEditor.StunForNextTurn(entity);
            _stunStatusEditor.StunForNextTurn(entity);

            Assert.IsTrue(_stunStatusEditor.ConsumeStun(entity));
            Assert.IsFalse(_stunStatusEditor.ConsumeStun(entity));
        }

        private IEntityView CreateEntity()
        {
            return _entityFactory.CreateEntity(
                new EntityDefinition(
                    maxHealth: 100,
                    maxEnergy: 10,
                    energyRegenerationPerTurn: 3,
                    speed: 1,
                    strength: 2));
        }
    }
}