using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : Singleton<GameManager>
{

    public HashSet<Enemy> Enemies;
    public Tilemap tilemapWalls;
    public Settings settings;

    private GameField[,] map;
    private readonly ConcurrentDictionary<Enemy, Vector3Int> enemyPositions = new ConcurrentDictionary<Enemy, Vector3Int>();
    private Vector3Int playerPosition;

    private void Awake()
    {
        if (settings == null)
            settings = ScriptableObject.CreateInstance<Settings>();
        var size = tilemapWalls.size;
        settings.widthMap = size.x;
        settings.heightMap = size.y;
        map = new GameField[settings.widthMap, settings.heightMap];
        Enemies = new HashSet<Enemy>(FindObjectsOfType<Enemy>());
        InitMap();
    }

    public void ProceedDamage(IUnit attacker, IUnit attacked, Damage damage)
    {
        attacked.GetDamage(damage, attacker);
        if (attacked.Health <= 0 && attacked.IDead == false)
        {
            Debug.Log(attacked);
            Debug.Log(attacker);    
            ExperienceSystem.Instance.CalculateExperience(attacker, attacked);
            attacked.IDead = true;
            attacked.Dead();
            Statistics.mobsKilled++;
        }
    }

    private void FixedUpdate()
    {
        UpdateMap(Enemies);
        foreach (var enemy in Enemies.Where(e => e.CanMove))
        {
            if (enemy.TryGetComponent<MoveAI>(out var moveComponent))
            {
                var enemyPosition = enemyPositions[enemy];
                moveComponent.UpdateAI(map, Player.Instance, (enemyPosition.x, enemyPosition.y));
            }
        }
    }

    private void InitMap()
    {
        for (int x = tilemapWalls.origin.x, i = 0; i < tilemapWalls.size.x; x++, i++)
        for (int y = tilemapWalls.origin.y, j = 0; j < tilemapWalls.size.y; y++, j++)
        {
            var tileWall = tilemapWalls.GetTile(
                new Vector3Int(x, y, 0)
            );

            map[i, j] = tileWall is null ? GameField.Empty : GameField.Wall;
        }

        UpdateMap(Enemies);
    }

    private void UpdateMap(IEnumerable<Enemy> enemies)
    {
        
        foreach (var enemy in enemies)
        {
            var tilePos = tilemapWalls.WorldToCell(enemy.transform.position) - tilemapWalls.origin;
            var initialized = enemyPositions.TryGetValue(enemy, out var enemyPosition);
            if (initialized && map[enemyPosition.x, enemyPosition.y] == GameField.Enemy)
            {
                map[enemyPosition.x, enemyPosition.y] = GameField.Empty;
            }

            enemyPositions[enemy] = tilePos;
            map[tilePos.x, tilePos.y] = GameField.Enemy;
        }
        var tilePlayer = tilemapWalls.WorldToCell(Player.Instance.transform.position) - tilemapWalls.origin;
        if (map[playerPosition.x, playerPosition.y] == GameField.Player)
        {
            map[playerPosition.x, playerPosition.y] = GameField.Empty;
        }

        map[tilePlayer.x, tilePlayer.y] = GameField.Player;
        playerPosition = tilePlayer;
    }
}


public enum GameField
{
    Empty,
    Player,
    Enemy,
    Wall,
}