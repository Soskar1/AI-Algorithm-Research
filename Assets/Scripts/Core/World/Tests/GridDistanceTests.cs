using AiAlgorithmsResearch.Core.Worlds.Api;
using NUnit.Framework;
using UnityEngine;

namespace AiAlgorithmsResearch.Core.Worlds.Tests
{
    public sealed class GridDistanceTests
    {
        [Test]
        public void Manhattan_WhenPositionsAreSame_ReturnsZero()
        {
            var distance = GridDistance.Manhattan(
                new Vector2Int(1, 2),
                new Vector2Int(1, 2));

            Assert.AreEqual(0, distance);
        }

        [Test]
        public void Manhattan_WhenPositionsAreDifferent_ReturnsManhattanDistance()
        {
            var distance = GridDistance.Manhattan(
                new Vector2Int(1, 2),
                new Vector2Int(4, 6));

            Assert.AreEqual(7, distance);
        }

        [Test]
        public void Manhattan_WhenPositionsContainNegativeCoordinates_ReturnsManhattanDistance()
        {
            var distance = GridDistance.Manhattan(
                new Vector2Int(-2, -3),
                new Vector2Int(4, 1));

            Assert.AreEqual(10, distance);
        }
    }
}