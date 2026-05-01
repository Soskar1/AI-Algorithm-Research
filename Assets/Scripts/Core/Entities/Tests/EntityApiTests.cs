using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Entities.Application;
using AiAlgorithmsResearch.Core.Entities.Domain;
using NUnit.Framework;

namespace AiAlgorithmsResearch.Core.Entities.Tests
{
    public sealed class EntityApiTests
    {
        private IEntityView _entity;
        private IEntityEnergyEditor _energyEditor;
        private IEntityHealthEditor _healthEditor;

        [SetUp]
        public void SetUp()
        {
            IEntityFactory entityFactory = new EntityFactory();

            _entity = entityFactory.CreateEntity(
                new EntityDefinition(
                    maxHealth: 100,
                    maxEnergy: 10,
                    energyRegenerationPerTurn: 3,
                    speed: 1,
                    strength: 2));

            _energyEditor = new EntityEnergyEditor();
            _healthEditor = new EntityHealthEditor();
        }

        [Test]
        public void Entity_WhenCreated_HasExpectedStats()
        {
            Assert.AreEqual(100, _entity.Health.Current);
            Assert.AreEqual(100, _entity.Health.Max);

            Assert.AreEqual(10, _entity.Energy.Current);
            Assert.AreEqual(10, _entity.Energy.Max);
            Assert.AreEqual(3, _entity.Energy.RegenerationPerTurn);

            Assert.AreEqual(1, _entity.Speed);
            Assert.AreEqual(2, _entity.Strength);
        }

        [Test]
        public void TrySpendEnergy_WhenEnoughEnergy_SpendsEnergy()
        {
            var result = _energyEditor.TrySpendEnergy(_entity, 4);

            Assert.IsTrue(result);
            Assert.AreEqual(6, _entity.Energy.Current);
        }

        [Test]
        public void TrySpendEnergy_WhenNotEnoughEnergy_ReturnsFalseAndDoesNotChangeEnergy()
        {
            var result = _energyEditor.TrySpendEnergy(_entity, 11);

            Assert.IsFalse(result);
            Assert.AreEqual(10, _entity.Energy.Current);
        }

        [Test]
        public void TrySpendEnergy_WhenAmountIsNegative_ReturnsFalseAndDoesNotChangeEnergy()
        {
            var result = _energyEditor.TrySpendEnergy(_entity, -1);

            Assert.IsFalse(result);
            Assert.AreEqual(10, _entity.Energy.Current);
        }

        [Test]
        public void RegenerateEnergy_WhenEnergyIsMissing_RegeneratesByAmount()
        {
            _energyEditor.TrySpendEnergy(_entity, 5);

            _energyEditor.RegenerateEnergy(_entity);

            Assert.AreEqual(8, _entity.Energy.Current);
        }

        [Test]
        public void RegenerateEnergy_WhenEnergyWouldExceedMax_ClampsToMax()
        {
            _energyEditor.TrySpendEnergy(_entity, 1);

            _energyEditor.RegenerateEnergy(_entity);

            Assert.AreEqual(10, _entity.Energy.Current);
        }

        [Test]
        public void DealDamage_WhenDamageIsLessThanCurrentHealth_ReducesHealth()
        {
            _healthEditor.DealDamage(_entity, 25);

            Assert.AreEqual(75, _entity.Health.Current);
        }

        [Test]
        public void DealDamage_WhenDamageExceedsCurrentHealth_ClampsToZero()
        {
            _healthEditor.DealDamage(_entity, 150);

            Assert.AreEqual(0, _entity.Health.Current);
        }

        [Test]
        public void Heal_WhenHealthIsMissing_RestoresHealth()
        {
            _healthEditor.DealDamage(_entity, 40);

            _healthEditor.Heal(_entity, 15);

            Assert.AreEqual(75, _entity.Health.Current);
        }

        [Test]
        public void Heal_WhenHealingWouldExceedMax_ClampsToMax()
        {
            _healthEditor.DealDamage(_entity, 10);

            _healthEditor.Heal(_entity, 50);

            Assert.AreEqual(100, _entity.Health.Current);
        }

        [Test]
        public void DealDamage_WhenAmountIsNegative_Throws()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() =>
                _healthEditor.DealDamage(_entity, -1));
        }

        [Test]
        public void Heal_WhenAmountIsNegative_Throws()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() =>
                _healthEditor.Heal(_entity, -1));
        }
    }
}