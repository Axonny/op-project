using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;


public class MoveAI : MonoBehaviour
{
    public float distanceRaycast = 5f;
    public float speed = 3f;
    private new Rigidbody2D rigidbody;
    private bool isDetectedPlayer;
    private static int minDistance = 13;
    public bool hasKey = false;
    [SerializeField] private LayerMask layerMask;

    private static readonly Moves[] Moves = new[]
    {
        new Moves(MoveType.Down, new Vector2Int(1, 0)),
        new Moves(MoveType.Up, new Vector2Int(-1, 0)),
        new Moves(MoveType.Right, new Vector2Int(0, 1)),
        new Moves(MoveType.Left, new Vector2Int(0, -1)),
    };

    private void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
    }

    public void UpdateAI(GameField[,] map, Player player, (int, int) enemyPosInMap, (int, int) playerPos)
    {
        if (isDetectedPlayer || TryDetectPlayer(player, out isDetectedPlayer))
            if (hasKey)
            {
                MoveAwayFromPlayer(map, enemyPosInMap, playerPos);
            }
            else
            {
                MoveToPlayer(map, enemyPosInMap, playerPos);
            }
    }

    private static List<(int, int)> FindPathAwayFromPlayer(GameField[,] map, (int, int) start, (int, int) playerPos)
    {
        var n = map.GetLength(0);
        var m = map.GetLength(1);
        var visited = new bool[n][];
        for (int index = 0; index < n; index++)
        {
            visited[index] = new bool[m];
        }

        var prev = new (int, int)[n][];
        for (int index = 0; index < n; index++)
        {
            prev[index] = new (int, int)[m];
        }

        var queue = new Queue<(int, int, int)>();
        queue.Enqueue((playerPos.Item1, playerPos.Item2, 0));
        var goodPoints = new HashSet<(int, int)>();
        while (queue.Count > 0)
        {
            var (x, y, length) = queue.Dequeue();
            if (length > minDistance)
            {
                break;
            }

            if (length == minDistance)
            {
                goodPoints.Add((x, y));
            }

            foreach (var move in Moves)
            {
                var newX = x + move.Move.y;
                var newY = y + move.Move.x;
                if (newX >= 0 && newX < n &&
                    newY >= 0 && newY < m &&
                    !visited[newX][newY] && map[newX, newY] != GameField.Wall)
                {
                    prev[newX][newY] = (x, y);
                    visited[newX][newY] = true;
                    queue.Enqueue((newX, newY, length + 1));
                }
            }
        }

        return MoveBFSOnTilemapToPoints(map, start, goodPoints);
    }

    private static List<(int, int)> MoveBFSOnTilemapToPoints(GameField[,] map, (int, int) start,
        HashSet<(int, int)> toPoints)
    {
        var n = map.GetLength(0);
        var m = map.GetLength(1);
        var visited = new bool[n][];
        for (int index = 0; index < n; index++)
        {
            visited[index] = new bool[m];
        }

        var prev = new (int, int)[n][];
        for (int index = 0; index < n; index++)
        {
            prev[index] = new (int, int)[m];
        }

        var queue = new Queue<(int, int)>();
        queue.Enqueue(start);
        var toPoint = (-1, -1);
        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();
            if (toPoints.Contains((x, y)))
            {
                toPoint = (x, y);
                break;
            }

            foreach (var move in Moves)
            {
                var newX = x + move.Move.y;
                var newY = y + move.Move.x;
                if (newX >= 0 && newX < n &&
                    newY >= 0 && newY < m &&
                    !visited[newX][newY] && map[newX, newY] != GameField.Wall)
                {
                    prev[newX][newY] = (x, y);
                    visited[newX][newY] = true;
                    queue.Enqueue((newX, newY));
                }
            }
        }

        var ans = new List<(int, int)>();
        if (toPoint == (-1, -1))
        {
            return new List<(int, int)>();
        }

        while (toPoint != start)
        {
            ans.Add(toPoint);
            toPoint = prev[toPoint.Item1][toPoint.Item2];
        }

        ans.Reverse();
        return ans;
    }

    private void MoveToPlayer(GameField[,] map, (int, int) enemyPosInMap, (int, int) playerPos)
    {
        var pathPoints = MoveBFSOnTilemapToPoints(map, enemyPosInMap, new HashSet<(int, int)>
        {
            playerPos
        });
        GoThroughPath(pathPoints, enemyPosInMap);
    }

    private void MoveAwayFromPlayer(GameField[,] map, (int, int) enemyPosInMap, (int, int) playerPos)
    {
        var pathPoints = FindPathAwayFromPlayer(map, enemyPosInMap, playerPos);
        GoThroughPath(pathPoints, enemyPosInMap);
    }

    private void GoThroughPath(List<(int, int)> pathPoints, (int, int) fromPos)
    {
        if (pathPoints.Count == 0)
        {
            return;
        }

        var i = 0;
        var currentVariant = pathPoints[i];
        while (true)
        {
            var from = GameManager.Instance.tilemapWalls.CellToWorld(
                new Vector3Int(fromPos.Item1, fromPos.Item2, 0) +
                GameManager.Instance.tilemapWalls.origin);
            var to = GameManager.Instance.tilemapWalls.CellToWorld(new Vector3Int(currentVariant.Item1,
                currentVariant.Item2, 0) + GameManager.Instance.tilemapWalls.origin);
            var offset = 0.5f / 2;
            var rayDist = Vector3.Distance(from, to);
            if (rayDist - 1 < 0.01f)
            {
                rayDist = 0.5f;
            }
            else
            {
                rayDist = rayDist - 0.6f;
            }

            if (TryThrowRayCast(transform.position - new Vector3(offset, -offset, 0), to - from,
                    rayDist) &&
                TryThrowRayCast(transform.position - new Vector3(offset, offset, 0), to - from,
                    rayDist) &&
                TryThrowRayCast(transform.position - new Vector3(-offset, -offset, 0), to - from,
                    rayDist) &&
                TryThrowRayCast(transform.position - new Vector3(-offset, offset, 0), to - from,
                    rayDist) &&
                i < pathPoints.Count - 1)
            {
                currentVariant = pathPoints[++i];
                rigidbody.velocity = (to - from).normalized * speed;
            }
            else
            {
                if (i == 0)
                {
                    var to2 = GameManager.Instance.tilemapWalls.CellToWorld(
                        new Vector3Int(fromPos.Item1, fromPos.Item2, 0) +
                        GameManager.Instance.tilemapWalls.origin) + new Vector3(0.5f, 0.5f, 0);
                    rigidbody.velocity = (to2 - transform.position)
                        .normalized * speed;
                }
                else if (i == pathPoints.Count - 1)
                {
                    rigidbody.velocity = (to - from).normalized * speed;
                }

                return;
            }
        }
    }

    private bool TryDetectPlayer(Player player, out bool result)
    {
        result = TryThrowRayCast(transform, player.transform, distanceRaycast);
        return result;
    }

    private bool TryThrowRayCast(Transform from, Transform to, float distance)
    {
        var position = from.position;

        var hit = Physics2D.Raycast(position, to.position - position, distance, layerMask);
        return hit.transform == to;
    }

    private bool TryThrowRayCast(Vector3 position, Vector3 direction, float distance)
    {
        var hit = Physics2D.Raycast(position, direction, distance, layerMask);
        return hit.transform is null;
    }
}