using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Combat.Domain;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Entities.Application;
using NUnit.Framework;

namespace AiAlgorithmsResearch.Core.Combat.Tests
{
    public sealed class ActionCooldownsTests
    {
        private IEntityFactory _entityFactory;
        private IActionCooldowns _cooldowns;
        private IActionCooldownEditor _cooldownEditor;

        [SetUp]
        public void SetUp()
        {
            _entityFactory = new EntityFactory();

            var actionCooldowns = new ActionCooldowns();
            _cooldowns = actionCooldowns;
            _cooldownEditor = actionCooldowns;
        }

        [Test]
        public void GetRemainingCooldown_WhenActionHasNoCooldown_ReturnsZero()
        {
            var entity = CreateEntity();

            var remaining = _cooldowns.GetRemainingCooldown(entity, CombatActionIds.Teleport);

            Assert.AreEqual(0, remaining);
            Assert.IsFalse(_cooldowns.IsOnCooldown(entity, CombatActionIds.Teleport));
        }

        [Test]
        public void PutOnCooldown_WhenTurnsArePositive_SetsCooldown()
        {
            var entity = CreateEntity();

            _cooldownEditor.PutOnCooldown(entity, CombatActionIds.Teleport, turns: 4);

            Assert.IsTrue(_cooldowns.IsOnCooldown(entity, CombatActionIds.Teleport));
            Assert.AreEqual(4, _cooldowns.GetRemainingCooldown(entity, CombatActionIds.Teleport));
        }

        [Test]
        public void PutOnCooldown_WhenTurnsAreZero_DoesNotSetCooldown()
        {
            var entity = CreateEntity();

            _cooldownEditor.PutOnCooldown(entity, CombatActionIds.Teleport, turns: 0);

            Assert.IsFalse(_cooldowns.IsOnCooldown(entity, CombatActionIds.Teleport));
            Assert.AreEqual(0, _cooldowns.GetRemainingCooldown(entity, CombatActionIds.Teleport));
        }

        [Test]
        public void PutOnCooldown_WhenTurnsAreNegative_DoesNotSetCooldown()
        {
            var entity = CreateEntity();

            _cooldownEditor.PutOnCooldown(entity, CombatActionIds.Teleport, turns: -1);

            Assert.IsFalse(_cooldowns.IsOnCooldown(entity, CombatActionIds.Teleport));
            Assert.AreEqual(0, _cooldowns.GetRemainingCooldown(entity, CombatActionIds.Teleport));
        }

        [Test]
        public void TickCooldowns_WhenCooldownExists_DecreasesCooldownByOne()
        {
            var entity = CreateEntity();

            _cooldownEditor.PutOnCooldown(entity, CombatActionIds.Teleport, turns: 4);

            _cooldownEditor.TickCooldowns(entity);

            Assert.AreEqual(3, _cooldowns.GetRemainingCooldown(entity, CombatActionIds.Teleport));
        }

        [Test]
        public void TickCooldowns_WhenCooldownReachesZero_RemovesCooldown()
        {
            var entity = CreateEntity();

            _cooldownEditor.PutOnCooldown(entity, CombatActionIds.Heal, turns: 1);

            _cooldownEditor.TickCooldowns(entity);

            Assert.IsFalse(_cooldowns.IsOnCooldown(entity, CombatActionIds.Heal));
            Assert.AreEqual(0, _cooldowns.GetRemainingCooldown(entity, CombatActionIds.Heal));
        }

        [Test]
        public void TickCooldowns_WhenEntityHasMultipleCooldowns_DecreasesAllCooldowns()
        {
            var entity = CreateEntity();

            _cooldownEditor.PutOnCooldown(entity, CombatActionIds.Teleport, turns: 4);
            _cooldownEditor.PutOnCooldown(entity, CombatActionIds.Heal, turns: 3);

            _cooldownEditor.TickCooldowns(entity);

            Assert.AreEqual(3, _cooldowns.GetRemainingCooldown(entity, CombatActionIds.Teleport));
            Assert.AreEqual(2, _cooldowns.GetRemainingCooldown(entity, CombatActionIds.Heal));
        }

        [Test]
        public void TickCooldowns_WhenOtherEntityHasCooldown_DoesNotAffectOtherEntity()
        {
            var firstEntity = CreateEntity();
            var secondEntity = CreateEntity();

            _cooldownEditor.PutOnCooldown(firstEntity, CombatActionIds.Teleport, turns: 4);
            _cooldownEditor.PutOnCooldown(secondEntity, CombatActionIds.Teleport, turns: 4);

            _cooldownEditor.TickCooldowns(firstEntity);

            Assert.AreEqual(3, _cooldowns.GetRemainingCooldown(firstEntity, CombatActionIds.Teleport));
            Assert.AreEqual(4, _cooldowns.GetRemainingCooldown(secondEntity, CombatActionIds.Teleport));
        }

        [Test]
        public void PutOnCooldown_WhenCooldownAlreadyExists_ReplacesCooldown()
        {
            var entity = CreateEntity();

            _cooldownEditor.PutOnCooldown(entity, CombatActionIds.Teleport, turns: 4);
            _cooldownEditor.PutOnCooldown(entity, CombatActionIds.Teleport, turns: 2);

            Assert.AreEqual(2, _cooldowns.GetRemainingCooldown(entity, CombatActionIds.Teleport));
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