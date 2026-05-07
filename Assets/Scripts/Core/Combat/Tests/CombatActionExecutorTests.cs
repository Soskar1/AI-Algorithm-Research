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
using System.Collections.Generic;
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
            

            var handlers = new Dictionary<CombatActionId, ICombatActionHandler>
            {
                [CombatActionIds.Wait] = new WaitActionHandler(logger),
                [CombatActionIds.Move] = new MoveActionHandler(logger),
                [CombatActionIds.Attack] = new AttackActionHandler(logger),
                [CombatActionIds.Teleport] = new TeleportActionHandler(logger),
                [CombatActionIds.Heal] = new HealActionHandler(logger),
                [CombatActionIds.Stun] = new StunActionHandler(logger)
            };

            _executor = new CombatActionExecutor(handlers);
        }

        [Test]
        public void TryExecute_WaitAction_ReturnsTrueAndDoesNotSpendEnergy()
        {
            var actor = CreateEntity();
            var battle = new Battle(new List<BattleParticipant>() { actor });
            var combatState = new RuntimeCombatState(_worldView, _worldEditor, _healthEditor, _energyEditor, _cooldowns, _cooldownEditor, _stunStatusEditor, _stunStatus, battle);

            AddEntity(actor.Entity, new Vector2Int(1, 1));

            var result = _executor.TryExecute(new WaitAction(actor.Entity.Id), combatState, combatState);

            Assert.IsTrue(result);
            Assert.AreEqual(10, actor.Entity.Energy.Current);
        }

        [Test]
        public void TryExecute_MoveAction_WhenActorHasEnoughEnergy_MovesEntityAndSpendsEnergy()
        {
            var actor = CreateEntity();
            var start = new Vector2Int(1, 1);
            var target = new Vector2Int(3, 1);

            AddEntity(actor.Entity, start);
            AddFreeTile(target);

            var battle = new Battle(new List<BattleParticipant>() { actor });
            var combatState = new RuntimeCombatState(_worldView, _worldEditor, _healthEditor, _energyEditor, _cooldowns, _cooldownEditor, _stunStatusEditor, _stunStatus, battle);
            var result = _executor.TryExecute(new MoveAction(actor.Entity.Id, target, 1), combatState, combatState);

            Assert.IsTrue(result);
            Assert.IsTrue(_worldView.TryGetEntityPosition(actor.Entity, out var position));
            Assert.AreEqual(target, position);
            Assert.AreEqual(9, actor.Entity.Energy.Current);
        }

        [Test]
        public void TryExecute_MoveAction_WhenActorDoesNotHaveEnoughEnergy_ReturnsFalseAndDoesNotMove()
        {
            var actor = CreateEntity(maxEnergy: 1);
            var start = new Vector2Int(1, 1);
            var target = new Vector2Int(5, 1);

            AddEntity(actor.Entity, start);
            AddFreeTile(target);

            var battle = new Battle(new List<BattleParticipant>() { actor });
            var combatState = new RuntimeCombatState(_worldView, _worldEditor, _healthEditor, _energyEditor, _cooldowns, _cooldownEditor, _stunStatusEditor, _stunStatus, battle);
            var result = _executor.TryExecute(new MoveAction(actor.Entity.Id, target, 2), combatState, combatState);

            Assert.IsFalse(result);
            Assert.IsTrue(_worldView.TryGetEntityPosition(actor.Entity, out var position));
            Assert.AreEqual(start, position);
            Assert.AreEqual(1, actor.Entity.Energy.Current);
        }

        [Test]
        public void TryExecute_AttackAction_WhenTargetIsInRange_DealsDamageAndSpendsEnergy()
        {
            var actor = CreateEntity(strength: 2);
            var target = CreateEntity();

            AddEntity(actor.Entity, new Vector2Int(1, 1));
            AddEntity(target.Entity, new Vector2Int(2, 1));

            var battle = new Battle(new List<BattleParticipant>() { actor, target });
            var combatState = new RuntimeCombatState(_worldView, _worldEditor, _healthEditor, _energyEditor, _cooldowns, _cooldownEditor, _stunStatusEditor, _stunStatus, battle);

            var result = _executor.TryExecute(
                new AttackAction(actor.Entity.Id, target.Entity.Id, baseDamage: 5, range: 1, cost: 2), combatState, combatState);

            Assert.IsTrue(result);
            Assert.AreEqual(93, target.Entity.Health.Current);
            Assert.AreEqual(8, actor.Entity.Energy.Current);
        }

        [Test]
        public void TryExecute_AttackAction_WhenTargetIsOutOfRange_ReturnsFalseAndDoesNotSpendEnergy()
        {
            var actor = CreateEntity(strength: 2);
            var target = CreateEntity();

            AddEntity(actor.Entity, new Vector2Int(1, 1));
            AddEntity(target.Entity, new Vector2Int(4, 1));

            var battle = new Battle(new List<BattleParticipant>() { actor, target });
            var combatState = new RuntimeCombatState(_worldView, _worldEditor, _healthEditor, _energyEditor, _cooldowns, _cooldownEditor, _stunStatusEditor, _stunStatus, battle);

            var result = _executor.TryExecute(
                new AttackAction(actor.Entity.Id, target.Entity.Id, baseDamage: 5, range: 1, cost: 2), combatState, combatState);

            Assert.IsFalse(result);
            Assert.AreEqual(100, target.Entity.Health.Current);
            Assert.AreEqual(10, actor.Entity.Energy.Current);
        }

        [Test]
        public void TryExecute_TeleportAction_MovesEntitySpendsEnergyAndAppliesCooldown()
        {
            var actor = CreateEntity();
            var start = new Vector2Int(1, 1);
            var target = new Vector2Int(5, 5);

            AddEntity(actor.Entity, start);
            AddFreeTile(target);

            var battle = new Battle(new List<BattleParticipant>() { actor });
            var combatState = new RuntimeCombatState(_worldView, _worldEditor, _healthEditor, _energyEditor, _cooldowns, _cooldownEditor, _stunStatusEditor, _stunStatus, battle);

            var result = _executor.TryExecute(new TeleportAction(actor.Entity.Id, target, cost: 2, cooldown: 4), combatState, combatState);

            Assert.IsTrue(result);
            Assert.IsTrue(_worldView.TryGetEntityPosition(actor.Entity, out var position));
            Assert.AreEqual(target, position);
            Assert.AreEqual(8, actor.Entity.Energy.Current);
            Assert.AreEqual(4, _cooldowns.GetRemainingCooldown(actor.Entity.Id, CombatActionIds.Teleport));
        }

        [Test]
        public void TryExecute_TeleportAction_WhenOnCooldown_ReturnsFalse()
        {
            var actor = CreateEntity();
            var start = new Vector2Int(1, 1);
            var target = new Vector2Int(5, 5);

            AddEntity(actor.Entity, start);
            AddFreeTile(target);

            _cooldownEditor.PutOnCooldown(actor.Entity.Id, CombatActionIds.Teleport, 4);

            var battle = new Battle(new List<BattleParticipant>() { actor });
            var combatState = new RuntimeCombatState(_worldView, _worldEditor, _healthEditor, _energyEditor, _cooldowns, _cooldownEditor, _stunStatusEditor, _stunStatus, battle);

            var result = _executor.TryExecute(new TeleportAction(actor.Entity.Id, target, cost: 1, cooldown: 1), combatState, combatState);

            Assert.IsFalse(result);
            Assert.IsTrue(_worldView.TryGetEntityPosition(actor.Entity, out var position));
            Assert.AreEqual(start, position);
            Assert.AreEqual(10, actor.Entity.Energy.Current);
        }

        [Test]
        public void TryExecute_HealAction_WhenHealthIsMissing_HealsSpendsEnergyAndAppliesCooldown()
        {
            var actor = CreateEntity();

            AddEntity(actor.Entity, new Vector2Int(1, 1));
            _healthEditor.DealDamage(actor.Entity, 20);

            var battle = new Battle(new List<BattleParticipant>() { actor });
            var combatState = new RuntimeCombatState(_worldView, _worldEditor, _healthEditor, _energyEditor, _cooldowns, _cooldownEditor, _stunStatusEditor, _stunStatus, battle);

            var result = _executor.TryExecute(new HealAction(actor.Entity.Id, cost: 2, cooldown: 3), combatState, combatState);

            Assert.IsTrue(result);
            Assert.AreEqual(85, actor.Entity.Health.Current);
            Assert.AreEqual(8, actor.Entity.Energy.Current);
            Assert.AreEqual(3, _cooldowns.GetRemainingCooldown(actor.Entity.Id, CombatActionIds.Heal));
        }

        [Test]
        public void TryExecute_HealAction_WhenHealthIsFull_ReturnsFalseAndDoesNotSpendEnergy()
        {
            var actor = CreateEntity();

            AddEntity(actor.Entity, new Vector2Int(1, 1));

            var battle = new Battle(new List<BattleParticipant>() { actor });
            var combatState = new RuntimeCombatState(_worldView, _worldEditor, _healthEditor, _energyEditor, _cooldowns, _cooldownEditor, _stunStatusEditor, _stunStatus, battle);

            var result = _executor.TryExecute(new HealAction(actor.Entity.Id, cost: 2, cooldown: 1), combatState, combatState);

            Assert.IsFalse(result);
            Assert.AreEqual(100, actor.Entity.Health.Current);
            Assert.AreEqual(10, actor.Entity.Energy.Current);
        }

        [Test]
        public void TryExecute_StunAction_WhenTargetIsAdjacent_StunsTargetSpendsEnergyAndAppliesCooldown()
        {
            var actor = CreateEntity();
            var target = CreateEntity();

            AddEntity(actor.Entity, new Vector2Int(1, 1));
            AddEntity(target.Entity, new Vector2Int(2, 1));

            var battle = new Battle(new List<BattleParticipant>() { actor, target });
            var combatState = new RuntimeCombatState(_worldView, _worldEditor, _healthEditor, _energyEditor, _cooldowns, _cooldownEditor, _stunStatusEditor, _stunStatus, battle);

            var result = _executor.TryExecute(new StunAction(actor.Entity.Id, target.Entity.Id, cost: 2, cooldown: 3), combatState, combatState);

            Assert.IsTrue(result);
            Assert.IsTrue(_stunStatus.IsStunned(target.Entity.Id));
            Assert.AreEqual(8, actor.Entity.Energy.Current);
            Assert.AreEqual(3, _cooldowns.GetRemainingCooldown(actor.Entity.Id, CombatActionIds.Stun));
        }

        [Test]
        public void TryExecute_StunAction_WhenTargetIsNotAdjacent_ReturnsFalseAndDoesNotSpendEnergy()
        {
            var actor = CreateEntity();
            var target = CreateEntity();

            AddEntity(actor.Entity, new Vector2Int(1, 1));
            AddEntity(target.Entity, new Vector2Int(3, 1));

            var battle = new Battle(new List<BattleParticipant>() { actor, target });
            var combatState = new RuntimeCombatState(_worldView, _worldEditor, _healthEditor, _energyEditor, _cooldowns, _cooldownEditor, _stunStatusEditor, _stunStatus, battle);

            var result = _executor.TryExecute(new StunAction(actor.Entity.Id, target.Entity.Id, cost: 2, cooldown: 1), combatState, combatState);

            Assert.IsFalse(result);
            Assert.IsFalse(_stunStatus.IsStunned(target.Entity.Id));
            Assert.AreEqual(10, actor.Entity.Energy.Current);
        }

        private BattleParticipant CreateEntity(
            int maxHealth = 100,
            int maxEnergy = 10,
            int energyRegenerationPerTurn = 3,
            int speed = 1,
            int strength = 2)
        {
            var definition = new EntityDefinition(maxHealth, maxEnergy, energyRegenerationPerTurn, speed, strength);
            var entity = _entityFactory.CreateEntity(definition);

            var combatActionDefinitions = new List<ICombatActionDefinition>(); 

            var battleParticipant = new BattleParticipant(entity, 1, new TeamId(1), combatActionDefinitions);

            return battleParticipant;
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
            public string GetEntityDisplayName(Entities.Api.EntityId entityId)
            {
                return string.Empty;
            }

            public string GetEntityRepresentation(Entities.Api.EntityId entityId, ICombatStateView stateView)
            {
                return string.Empty;
            }

            public void Log(Entities.Api.EntityId entityId, string text, ICombatStateView stateView)
            {
                
            }
        }
    }
}