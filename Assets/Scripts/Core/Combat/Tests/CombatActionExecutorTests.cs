using System.Collections.Generic;
using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Combat.Application;
using AiAlgorithmsResearch.Core.Combat.Domain;
using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Entities.Application;
using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Maps.Application;
using AiAlgorithmsResearch.Core.Maps.Domain;
using AiAlgorithmsResearch.Core.Worlds.Api;
using AiAlgorithmsResearch.Core.Worlds.Application;
using AiAlgorithmsResearch.Core.Worlds.Domain;
using NUnit.Framework;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Combat.Tests
{
    public sealed class CombatActionExecutorTests
    {
        private IMapEditor _mapEditor;
        private IWorldView _worldView;
        private IWorldEditor _worldEditor;

        private IEntityFactory _entityFactory;
        private IEntityHealthEditor _healthEditor;
        private IEntityEnergyEditor _energyEditor;

        private IActionCooldowns _cooldowns;
        private IActionCooldownEditor _cooldownEditor;
        private IStunStatus _stunStatus;
        private IStunStatusEditor _stunStatusEditor;

        private ICombatActionExecutor _executor;

        [SetUp]
        public void SetUp()
        {
            var map = new Map();
            _mapEditor = new MapEditor(map);

            var world = new World(_mapEditor, map);
            _worldView = world;
            _worldEditor = new WorldEditor(world);

            _entityFactory = new EntityFactory();
            _healthEditor = new EntityHealthEditor();
            _energyEditor = new EntityEnergyEditor();

            var cooldowns = new ActionCooldowns();
            _cooldowns = cooldowns;
            _cooldownEditor = cooldowns;

            var stunStatus = new StunStatus();
            _stunStatus = stunStatus;
            _stunStatusEditor = stunStatus;

            var logger = new MockCombatLogger();
            var runtimeState = new RuntimeCombatState(world, _worldEditor, _healthEditor, _energyEditor, _cooldowns, _cooldownEditor, _stunStatusEditor);

            var handlers = new Dictionary<CombatActionId, ICombatActionHandler>
            {
                [CombatActionIds.Wait] = new WaitActionHandler(logger),
                [CombatActionIds.Move] = new MoveActionHandler(runtimeState, runtimeState, logger),
                [CombatActionIds.Attack] = new AttackActionHandler(runtimeState, runtimeState, logger),
                [CombatActionIds.Teleport] = new TeleportActionHandler(runtimeState, runtimeState, logger),
                [CombatActionIds.Heal] = new HealActionHandler(runtimeState, runtimeState, logger),
                [CombatActionIds.Stun] = new StunActionHandler(runtimeState, runtimeState, logger)
            };

            _executor = new CombatActionExecutor(handlers, runtimeState, runtimeState);
        }

        [Test]
        public void TryExecute_WaitAction_ReturnsTrueAndDoesNotSpendEnergy()
        {
            var actor = CreateEntity();

            var result = _executor.TryExecute(new WaitAction(actor));

            Assert.IsTrue(result);
            Assert.AreEqual(10, actor.Energy.Current);
        }

        [Test]
        public void TryExecute_MoveAction_WhenActorHasEnoughEnergy_MovesEntityAndSpendsEnergy()
        {
            var actor = CreateEntity();
            var start = new Vector2Int(1, 1);
            var target = new Vector2Int(3, 1);

            AddEntity(actor, start);
            AddFreeTile(target);

            var result = _executor.TryExecute(new MoveAction(actor, target, 1));

            Assert.IsTrue(result);
            Assert.IsTrue(_worldView.TryGetEntityPosition(actor, out var position));
            Assert.AreEqual(target, position);
            Assert.AreEqual(9, actor.Energy.Current);
        }

        [Test]
        public void TryExecute_MoveAction_WhenActorDoesNotHaveEnoughEnergy_ReturnsFalseAndDoesNotMove()
        {
            var actor = CreateEntity(maxEnergy: 1);
            var start = new Vector2Int(1, 1);
            var target = new Vector2Int(5, 1);

            AddEntity(actor, start);
            AddFreeTile(target);

            var result = _executor.TryExecute(new MoveAction(actor, target, 2));

            Assert.IsFalse(result);
            Assert.IsTrue(_worldView.TryGetEntityPosition(actor, out var position));
            Assert.AreEqual(start, position);
            Assert.AreEqual(1, actor.Energy.Current);
        }

        [Test]
        public void TryExecute_AttackAction_WhenTargetIsInRange_DealsDamageAndSpendsEnergy()
        {
            var actor = CreateEntity(strength: 2);
            var target = CreateEntity();

            AddEntity(actor, new Vector2Int(1, 1));
            AddEntity(target, new Vector2Int(2, 1));

            var result = _executor.TryExecute(
                new AttackAction(actor, target, baseDamage: 5, range: 1, cost: 2));

            Assert.IsTrue(result);
            Assert.AreEqual(93, target.Health.Current);
            Assert.AreEqual(8, actor.Energy.Current);
        }

        [Test]
        public void TryExecute_AttackAction_WhenTargetIsOutOfRange_ReturnsFalseAndDoesNotSpendEnergy()
        {
            var actor = CreateEntity(strength: 2);
            var target = CreateEntity();

            AddEntity(actor, new Vector2Int(1, 1));
            AddEntity(target, new Vector2Int(4, 1));

            var result = _executor.TryExecute(
                new AttackAction(actor, target, baseDamage: 5, range: 1, cost: 2));

            Assert.IsFalse(result);
            Assert.AreEqual(100, target.Health.Current);
            Assert.AreEqual(10, actor.Energy.Current);
        }

        [Test]
        public void TryExecute_TeleportAction_MovesEntitySpendsEnergyAndAppliesCooldown()
        {
            var actor = CreateEntity();
            var start = new Vector2Int(1, 1);
            var target = new Vector2Int(5, 5);

            AddEntity(actor, start);
            AddFreeTile(target);

            var result = _executor.TryExecute(new TeleportAction(actor, target, cost: 2, cooldown: 4));

            Assert.IsTrue(result);
            Assert.IsTrue(_worldView.TryGetEntityPosition(actor, out var position));
            Assert.AreEqual(target, position);
            Assert.AreEqual(8, actor.Energy.Current);
            Assert.AreEqual(4, _cooldowns.GetRemainingCooldown(actor, CombatActionIds.Teleport));
        }

        [Test]
        public void TryExecute_TeleportAction_WhenOnCooldown_ReturnsFalse()
        {
            var actor = CreateEntity();
            var start = new Vector2Int(1, 1);
            var target = new Vector2Int(5, 5);

            AddEntity(actor, start);
            AddFreeTile(target);

            _cooldownEditor.PutOnCooldown(actor, CombatActionIds.Teleport, 4);

            var result = _executor.TryExecute(new TeleportAction(actor, target, cost: 1, cooldown: 1));

            Assert.IsFalse(result);
            Assert.IsTrue(_worldView.TryGetEntityPosition(actor, out var position));
            Assert.AreEqual(start, position);
            Assert.AreEqual(10, actor.Energy.Current);
        }

        [Test]
        public void TryExecute_HealAction_WhenHealthIsMissing_HealsSpendsEnergyAndAppliesCooldown()
        {
            var actor = CreateEntity();

            _healthEditor.DealDamage(actor, 20);

            var result = _executor.TryExecute(new HealAction(actor, cost: 2, cooldown: 3));

            Assert.IsTrue(result);
            Assert.AreEqual(85, actor.Health.Current);
            Assert.AreEqual(8, actor.Energy.Current);
            Assert.AreEqual(3, _cooldowns.GetRemainingCooldown(actor, CombatActionIds.Heal));
        }

        [Test]
        public void TryExecute_HealAction_WhenHealthIsFull_ReturnsFalseAndDoesNotSpendEnergy()
        {
            var actor = CreateEntity();

            var result = _executor.TryExecute(new HealAction(actor, cost: 2, cooldown: 1));

            Assert.IsFalse(result);
            Assert.AreEqual(100, actor.Health.Current);
            Assert.AreEqual(10, actor.Energy.Current);
        }

        [Test]
        public void TryExecute_StunAction_WhenTargetIsAdjacent_StunsTargetSpendsEnergyAndAppliesCooldown()
        {
            var actor = CreateEntity();
            var target = CreateEntity();

            AddEntity(actor, new Vector2Int(1, 1));
            AddEntity(target, new Vector2Int(2, 1));

            var result = _executor.TryExecute(new StunAction(actor, target, cost: 2, cooldown: 3));

            Assert.IsTrue(result);
            Assert.IsTrue(_stunStatus.IsStunned(target));
            Assert.AreEqual(8, actor.Energy.Current);
            Assert.AreEqual(3, _cooldowns.GetRemainingCooldown(actor, CombatActionIds.Stun));
        }

        [Test]
        public void TryExecute_StunAction_WhenTargetIsNotAdjacent_ReturnsFalseAndDoesNotSpendEnergy()
        {
            var actor = CreateEntity();
            var target = CreateEntity();

            AddEntity(actor, new Vector2Int(1, 1));
            AddEntity(target, new Vector2Int(3, 1));

            var result = _executor.TryExecute(new StunAction(actor, target, cost: 2, cooldown: 1));

            Assert.IsFalse(result);
            Assert.IsFalse(_stunStatus.IsStunned(target));
            Assert.AreEqual(10, actor.Energy.Current);
        }

        private IEntityView CreateEntity(
            int maxHealth = 100,
            int maxEnergy = 10,
            int energyRegenerationPerTurn = 3,
            int speed = 1,
            int strength = 2)
        {
            return _entityFactory.CreateEntity(
                new EntityDefinition(
                    maxHealth,
                    maxEnergy,
                    energyRegenerationPerTurn,
                    speed,
                    strength));
        }

        private void AddEntity(IEntityView entity, Vector2Int position)
        {
            AddFreeTile(position);
            _worldEditor.TryAddEntity(entity, position);
        }

        private void AddFreeTile(Vector2Int position)
        {
            _mapEditor.AddTile(position, MapNodeType.Free);
        }

        private class MockCombatLogger : ICombatLogger
        {
            public string GetEntityIdString(IEntityView entity)
            {
                return string.Empty;
            }

            public string GetEntityRepresentation(IEntityView entity)
            {
                return string.Empty;
            }

            public void Log(IEntityView entity, string text)
            {

            }
        }
    }
}