using AiAlgorithmsResearch.Core.Combat.Api;
using AiAlgorithmsResearch.Core.Combat.Application;
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
    public sealed class BattleInitializerTests
    {
        private IWorldEditor _worldEditor;
        private IMapEditor _mapEditor;
        private FakeInitiativeRoller _initiativeRoller;
        private IBattleInitializer _battleInitializer;
        private IEntityFactory _entityFactory;

        [SetUp]
        public void SetUp()
        {
            var map = new Map();
            _mapEditor = new MapEditor(map);

            var world = new World(_mapEditor, map);
            _worldEditor = new WorldEditor(world);

            _initiativeRoller = new FakeInitiativeRoller();
            _battleInitializer = new BattleInitializer(_worldEditor, _initiativeRoller);

            _entityFactory = new EntityFactory();
        }

        [Test]
        public void StartBattle_WhenAllParticipantsCanSpawn_ReturnsBattle()
        {
            var entity = CreateEntity();
            var position = new Vector2Int(1, 1);

            AddFreeTile(position);
            _initiativeRoller.SetRoll(entity, 10);

            var request = new BattleInitializationRequest(
                new[]
                {
                    new BattleParticipantSetup(entity, position)
                });

            var battle = _battleInitializer.StartBattle(request);

            Assert.NotNull(battle);
            Assert.AreEqual(1, battle.TurnOrder.Count);
            Assert.AreSame(entity, battle.Current.Entity);
            Assert.AreEqual(10, battle.Current.Initiative);
        }

        [Test]
        public void StartBattle_WhenParticipantCannotSpawn_ReturnsNull()
        {
            var entity = CreateEntity();
            var blockedPosition = new Vector2Int(1, 1);

            _mapEditor.AddTile(blockedPosition, MapNodeType.Obstacle);

            var request = new BattleInitializationRequest(
                new[]
                {
                    new BattleParticipantSetup(entity, blockedPosition)
                });

            var battle = _battleInitializer.StartBattle(request);

            Assert.IsNull(battle);
        }

        [Test]
        public void StartBattle_WhenMultipleParticipantsCanSpawn_SortsTurnOrderByInitiativeDescending()
        {
            var slowEntity = CreateEntity();
            var fastEntity = CreateEntity();

            var slowPosition = new Vector2Int(1, 1);
            var fastPosition = new Vector2Int(2, 1);

            AddFreeTile(slowPosition);
            AddFreeTile(fastPosition);

            _initiativeRoller.SetRoll(slowEntity, 5);
            _initiativeRoller.SetRoll(fastEntity, 18);

            var request = new BattleInitializationRequest(
                new[]
                {
                    new BattleParticipantSetup(slowEntity, slowPosition),
                    new BattleParticipantSetup(fastEntity, fastPosition)
                });

            var battle = _battleInitializer.StartBattle(request);

            Assert.NotNull(battle);
            Assert.AreEqual(2, battle.TurnOrder.Count);

            Assert.AreSame(fastEntity, battle.TurnOrder[0].Entity);
            Assert.AreEqual(18, battle.TurnOrder[0].Initiative);

            Assert.AreSame(slowEntity, battle.TurnOrder[1].Entity);
            Assert.AreEqual(5, battle.TurnOrder[1].Initiative);

            Assert.AreSame(fastEntity, battle.Current.Entity);
        }

        [Test]
        public void StartBattle_WhenTwoParticipantsUseSameSpawnPosition_ReturnsNull()
        {
            var firstEntity = CreateEntity();
            var secondEntity = CreateEntity();

            var position = new Vector2Int(1, 1);

            AddFreeTile(position);

            var request = new BattleInitializationRequest(
                new[]
                {
                    new BattleParticipantSetup(firstEntity, position),
                    new BattleParticipantSetup(secondEntity, position)
                });

            var battle = _battleInitializer.StartBattle(request);

            Assert.IsNull(battle);
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

        private void AddFreeTile(Vector2Int position)
        {
            _mapEditor.AddTile(position, MapNodeType.Free);
        }

        private sealed class FakeInitiativeRoller : IInitiativeRoller
        {
            private readonly Dictionary<IEntityView, int> _rolls = new();

            public void SetRoll(IEntityView entity, int roll)
            {
                _rolls[entity] = roll;
            }

            public int Roll(IEntityView entity)
            {
                return _rolls[entity];
            }
        }
    }
}