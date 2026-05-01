using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Maps.Application;
using AiAlgorithmsResearch.Core.Maps.Domain;
using AiAlgorithmsResearch.Core.Worlds.Domain;
using NUnit.Framework;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Worlds.Tests
{
    public sealed class WorldViewTests
    {
        private World _world;

        [SetUp]
        public void SetUp()
        {
            var map = new Map();
            IMapEditor mapEditor = new MapEditor(map);

            _world = new World(mapEditor, map);
        }

        [Test]
        public void TryGetEntityPosition_WhenEntityExists_ReturnsTrueAndPosition()
        {
            var entity = new TestEntity();
            var position = new Vector2Int(3, 4);

            _world.AddEntity(entity, position);

            var result = _world.TryGetEntityPosition(entity, out var foundPosition);

            Assert.IsTrue(result);
            Assert.AreEqual(position, foundPosition);
        }

        [Test]
        public void TryGetEntityPosition_WhenEntityDoesNotExist_ReturnsFalseAndDefaultPosition()
        {
            var entity = new TestEntity();

            var result = _world.TryGetEntityPosition(entity, out var position);

            Assert.IsFalse(result);
            Assert.AreEqual(default(Vector2Int), position);
        }

        [Test]
        public void TryGetEntityPosition_WhenDifferentEntityExists_ReturnsFalse()
        {
            var existingEntity = new TestEntity();
            var searchedEntity = new TestEntity();

            _world.AddEntity(existingEntity, new Vector2Int(1, 1));

            var result = _world.TryGetEntityPosition(searchedEntity, out _);

            Assert.IsFalse(result);
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