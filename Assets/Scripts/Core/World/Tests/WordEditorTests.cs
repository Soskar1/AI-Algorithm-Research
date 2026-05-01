using AiAlgorithmsResearch.Core.Entities.Api;
using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Maps.Application;
using AiAlgorithmsResearch.Core.Maps.Domain;
using AiAlgorithmsResearch.Core.Worlds.Api;
using AiAlgorithmsResearch.Core.Worlds.Application;
using AiAlgorithmsResearch.Core.Worlds.Domain;
using NUnit.Framework;
using System.Linq;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Worlds.Tests
{
    public class WordEditorTests
    {
        private IWorldView _worldView;
        private IWorldEditor _worldEditor;
        private IMapEditor _mapEditor;

        [SetUp]
        public void SetUp()
        {
            var map = new Map();
            _mapEditor = new MapEditor(map);
            var world = new World(_mapEditor, map);

            _worldView = world;
            _worldEditor = new WorldEditor(world);
        }

        [Test]
        public void AddEntity_WhenTileExistsAndIsFree_AddsEntity()
        {
            var position = new Vector2Int(1, 1);
            var entity = new TestEntity();

            AddFreeTile(position);

            var result = _worldEditor.TryAddEntity(entity, position);

            Assert.IsTrue(result);
            Assert.AreEqual(1, _worldView.Entities.Count);

            var worldEntity = _worldView.Entities.Single();
            Assert.AreSame(entity, worldEntity.Entity);
            Assert.AreEqual(position, worldEntity.Position);
        }

        [Test]
        public void AddEntity_WhenTileDoesNotExist_ReturnsFalse()
        {
            var entity = new TestEntity();

            var result = _worldEditor.TryAddEntity(entity, new Vector2Int(99, 99));

            Assert.IsFalse(result);
            Assert.IsEmpty(_worldView.Entities);
        }

        [Test]
        public void AddEntity_WhenTileIsObstacle_ReturnsFalse()
        {
            var position = new Vector2Int(1, 1);
            var entity = new TestEntity();

            AddObstacleTile(position);

            var result = _worldEditor.TryAddEntity(entity, position);

            Assert.IsFalse(result);
            Assert.IsEmpty(_worldView.Entities);
        }

        [Test]
        public void AddEntity_WhenPositionIsOccupied_ReturnsFalse()
        {
            var position = new Vector2Int(1, 1);
            var firstEntity = new TestEntity();
            var secondEntity = new TestEntity();

            AddFreeTile(position);
            _worldEditor.TryAddEntity(firstEntity, position);

            var result = _worldEditor.TryAddEntity(secondEntity, position);

            Assert.IsFalse(result);
            Assert.AreEqual(1, _worldView.Entities.Count);
            Assert.AreSame(firstEntity, _worldView.Entities.Single().Entity);
        }

        [Test]
        public void MoveEntity_WhenTargetTileExistsAndIsFree_MovesEntity()
        {
            var start = new Vector2Int(1, 1);
            var target = new Vector2Int(2, 1);
            var entity = new TestEntity();

            AddFreeTile(start);
            AddFreeTile(target);
            _worldEditor.TryAddEntity(entity, start);

            var result = _worldEditor.TryMoveEntity(entity, target);

            Assert.IsTrue(result);

            var worldEntity = _worldView.Entities.Single();
            Assert.AreSame(entity, worldEntity.Entity);
            Assert.AreEqual(target, worldEntity.Position);
        }

        [Test]
        public void MoveEntity_WhenEntityDoesNotExist_ReturnsFalse()
        {
            var target = new Vector2Int(2, 1);
            var entity = new TestEntity();

            AddFreeTile(target);

            var result = _worldEditor.TryMoveEntity(entity, target);

            Assert.IsFalse(result);
            Assert.IsEmpty(_worldView.Entities);
        }

        [Test]
        public void MoveEntity_WhenTargetTileDoesNotExist_ReturnsFalse()
        {
            var start = new Vector2Int(1, 1);
            var target = new Vector2Int(99, 99);
            var entity = new TestEntity();

            AddFreeTile(start);
            _worldEditor.TryAddEntity(entity, start);

            var result = _worldEditor.TryMoveEntity(entity, target);

            Assert.IsFalse(result);
            Assert.AreEqual(start, _worldView.Entities.Single().Position);
        }

        [Test]
        public void MoveEntity_WhenTargetTileIsObstacle_ReturnsFalse()
        {
            var start = new Vector2Int(1, 1);
            var target = new Vector2Int(2, 1);
            var entity = new TestEntity();

            AddFreeTile(start);
            AddObstacleTile(target);
            _worldEditor.TryAddEntity(entity, start);

            var result = _worldEditor.TryMoveEntity(entity, target);

            Assert.IsFalse(result);
            Assert.AreEqual(start, _worldView.Entities.Single().Position);
        }

        [Test]
        public void MoveEntity_WhenTargetPositionIsOccupied_ReturnsFalse()
        {
            var firstPosition = new Vector2Int(1, 1);
            var secondPosition = new Vector2Int(2, 1);

            var firstEntity = new TestEntity();
            var secondEntity = new TestEntity();

            AddFreeTile(firstPosition);
            AddFreeTile(secondPosition);

            _worldEditor.TryAddEntity(firstEntity, firstPosition);
            _worldEditor.TryAddEntity(secondEntity, secondPosition);

            var result = _worldEditor.TryMoveEntity(firstEntity, secondPosition);

            Assert.IsFalse(result);

            Assert.AreEqual(firstPosition, _worldView.Entities.Single(e => ReferenceEquals(e.Entity, firstEntity)).Position);
            Assert.AreEqual(secondPosition, _worldView.Entities.Single(e => ReferenceEquals(e.Entity, secondEntity)).Position);
        }

        [Test]
        public void WorldView_ReturnsMapView()
        {
            var position = new Vector2Int(3, 4);

            AddFreeTile(position);

            var result = _worldView.Map.TryGetNode(position, out var node);

            Assert.IsTrue(result);
            Assert.AreEqual(position, node.Position);
            Assert.AreEqual(MapNodeType.Free, node.Type);
        }

        private void AddFreeTile(Vector2Int position)
        {
            _mapEditor.AddTile(position, MapNodeType.Free);
        }

        private void AddObstacleTile(Vector2Int position)
        {
            _mapEditor.AddTile(position, MapNodeType.Obstacle);
        }

        private sealed class TestEntity : IEntityView
        {
            public int Speed => throw new System.NotImplementedException();

            public int Strength => throw new System.NotImplementedException();

            IHealthView IEntityView.Health => throw new System.NotImplementedException();

            IEnergyView IEntityView.Energy => throw new System.NotImplementedException();
        }
    }
}