using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;

namespace TestProject1
{
    public class Tests
    {
        private GameField[,] FromLines(int[,] textMap)
        {
            var length = textMap.GetLength(0);
            var width = textMap.GetLength(1);
            var map = new GameField[length, width];
            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    map[x, y] = (GameField) textMap[x, y];
                }
            }

            return map;
        }

        [Test, Order(0)]
        public void ReturnNoPaths_WhenNoPathsToChests()
        {
            var textMap = new int[,]
            {
                {0, 3, 0},
                {3, 3, 0},
                {0, 0, 0},
            };
            var start = (0, 0);
            var map = FromLines(textMap);

            var paths = GetPaths(map, start);

            Assert.IsEmpty(paths);
        }


        [Test, Order(1)]
        public void ReturnCorrectPaths_OnEmptyDungeon()
        {
            var textMap = new int[,]
            {
                {2, 0, 1},
                {3, 3, 0},
                {0, 0, 0},
            };
            var start = (0, 0);
            var map = FromLines(textMap);

            var paths = GetPaths(map, start);

            AssertPath(map, paths, 2);
        }

        private static List<(int, int)> GetPaths(GameField[,] map, (int, int) start)
        {
            var paths = MoveAI.MoveBFSOnTilemapToPoint(map, start);
            return paths;
        }

        private void AssertPath(GameField[,] map, List<(int, int)> path, int expectedLength)
        {
            var directions = new List<(int, int)>
            {
                (1, 0), (0, 1), (-1, 0), (0, -1),
            };
            Assert.IsNotEmpty(path, "path should not be empty");
            for (var i = 0; i < path.Count - 1; i++)
            {
                var offset = (path[i].Item1 - path[i + 1].Item1, path[i].Item2 - path[i + 1].Item2);
                Assert.IsTrue(directions.Contains(offset),
                    $"Incorrect step #{i} in your path: from {path[i]} to {path[i + 1]}");
                Assert.AreNotEqual(GameField.Wall, map[path[i + 1].Item1, path[i + 1].Item2],
                    $"Collided with wall at {i}th path point: {path[i + 1]}");
            }

            Assert.AreEqual(GameField.Player, map[path.Last().Item1, path.Last().Item2],
                "The last point in path must be 'start'");
            Assert.GreaterOrEqual(path.Count, expectedLength,
                "Checker bug?! Leave a comment above this slide, to notify task authors, please");
            Assert.AreEqual(expectedLength, path.Count, "Your path is not the shortest one");
        }
    }
}