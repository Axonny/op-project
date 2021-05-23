using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;


public class MoveAI : MonoBehaviour
{
    public float distanceRaycast = 5f;
    public float speed = 3f;
    private new Rigidbody2D rigidbody;
    private bool isDetectedPlayer;
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

    public void UpdateAI(GameField[,] map, Player player, (int, int) enemyPosInMap)
    {
        if (isDetectedPlayer || TryDetectPlayer(player, out isDetectedPlayer))
            MoveToPlayer(map, enemyPosInMap);
    }

    private static List<(int, int)> MoveBFSOnTilemapToPoint(GameField[,] map, (int, int) start)
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
        (int, int) playerPosition = (-1, -1);
        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();
            if (map[x, y] == GameField.Player)
            {
                playerPosition = (x, y);
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
        if (playerPosition == (-1, -1))
        {
            return new List<(int, int)>();
        }

        while (playerPosition != start)
        {
            ans.Add(playerPosition);
            playerPosition = prev[playerPosition.Item1][playerPosition.Item2];
        }

        ans.Reverse();
        return ans;
    }

    private void MoveToPlayer(GameField[,] map, (int, int) enemyPosInMap)
    {
        var pathPoints = MoveBFSOnTilemapToPoint(map, enemyPosInMap);
        if (pathPoints.Count == 0)
        {
            return;
        }

        var i = 0;
        var currentVariant = pathPoints[i];
        while (true)
        {
            var from = GameManager.Instance.tilemapWalls.CellToWorld(
                new Vector3Int(enemyPosInMap.Item1, enemyPosInMap.Item2, 0) +
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
                        new Vector3Int(enemyPosInMap.Item1, enemyPosInMap.Item2, 0) +
                        GameManager.Instance.tilemapWalls.origin) + new Vector3(0.5f, 0.5f, 0);
                    rigidbody.velocity = (to2 - transform.position)
                        .normalized * speed;
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