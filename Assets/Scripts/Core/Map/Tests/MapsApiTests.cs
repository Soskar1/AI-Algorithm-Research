using AiAlgorithmsResearch.Core.Maps.Api;
using AiAlgorithmsResearch.Core.Maps.Application;
using AiAlgorithmsResearch.Core.Maps.Domain;
using NUnit.Framework;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Maps.Tests
{
    public class MapsApiTests
    {
        private IMapEditor _editor;
        private IReadOnlyTileMap _map;

        [SetUp]
        public void SetUp()
        {
            var map = new Map();

            _editor = new MapEditor(map);
            _map = map;
        }

        [Test]
        public void AddTile_WhenTileDoesNotExist_AddsTile()
        {
            var position = new Vector2Int(1, 2);

            var result = _editor.AddTile(position, MapNodeType.Free);

            Assert.IsTrue(result);
            Assert.IsTrue(_map.TryGetNode(position, out var node));
            Assert.AreEqual(position, node.Position);
            Assert.AreEqual(MapNodeType.Free, node.Type);
        }

        [Test]
        public void AddTile_WhenTileAlreadyExists_ReturnsFalse()
        {
            var position = new Vector2Int(1, 2);

            _editor.AddTile(position, MapNodeType.Free);

            var result = _editor.AddTile(position, MapNodeType.Obstacle);

            Assert.IsFalse(result);
            Assert.IsTrue(_map.TryGetNode(position, out var node));
            Assert.AreEqual(MapNodeType.Free, node.Type);
        }

        [Test]
        public void SetTileType_WhenTileExists_ChangesTileType()
        {
            var position = new Vector2Int(1, 2);

            _editor.AddTile(position, MapNodeType.Free);

            var result = _editor.SetTileType(position, MapNodeType.Obstacle);

            Assert.IsTrue(result);
            Assert.IsTrue(_map.TryGetNode(position, out var node));
            Assert.AreEqual(MapNodeType.Obstacle, node.Type);
        }

        [Test]
        public void SetTileType_WhenTileDoesNotExist_ReturnsFalse()
        {
            var result = _editor.SetTileType(
                new Vector2Int(1, 2),
                MapNodeType.Obstacle);

            Assert.IsFalse(result);
        }

        [Test]
        public void TryGetNode_WhenTileDoesNotExist_ReturnsFalse()
        {
            var result = _map.TryGetNode(new Vector2Int(99, 99), out var node);

            Assert.IsFalse(result);
            Assert.IsNull(node);
        }

        [Test]
        public void Connect_WhenBothTilesExist_ReturnsTrue()
        {
            var a = new Vector2Int(0, 0);
            var b = new Vector2Int(1, 0);

            _editor.AddTile(a, MapNodeType.Free);
            _editor.AddTile(b, MapNodeType.Free);

            var result = _editor.Connect(a, b);

            Assert.IsTrue(result);
        }

        [Test]
        public void Connect_WhenFirstTileDoesNotExist_ReturnsFalse()
        {
            var a = new Vector2Int(0, 0);
            var b = new Vector2Int(1, 0);

            _editor.AddTile(b, MapNodeType.Free);

            var result = _editor.Connect(a, b);

            Assert.IsFalse(result);
        }

        [Test]
        public void Connect_WhenSecondTileDoesNotExist_ReturnsFalse()
        {
            var a = new Vector2Int(0, 0);
            var b = new Vector2Int(1, 0);

            _editor.AddTile(a, MapNodeType.Free);

            var result = _editor.Connect(a, b);

            Assert.IsFalse(result);
        }

        [Test]
        public void Connect_WhenConnectionAlreadyExists_ReturnsFalse()
        {
            var a = new Vector2Int(0, 0);
            var b = new Vector2Int(1, 0);

            _editor.AddTile(a, MapNodeType.Free);
            _editor.AddTile(b, MapNodeType.Free);

            _editor.Connect(a, b);

            var result = _editor.Connect(a, b);

            Assert.IsFalse(result);
        }

        [Test]
        public void Connect_WhenConnectionAlreadyExistsInReverseOrder_ReturnsFalse()
        {
            var a = new Vector2Int(0, 0);
            var b = new Vector2Int(1, 0);

            _editor.AddTile(a, MapNodeType.Free);
            _editor.AddTile(b, MapNodeType.Free);

            _editor.Connect(a, b);

            var result = _editor.Connect(b, a);

            Assert.IsFalse(result);
        }
    }
}