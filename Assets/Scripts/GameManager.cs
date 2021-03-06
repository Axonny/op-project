using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GateScripts;
using Interfaces;
using PlayerScripts;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : Singleton<GameManager>
{

    public HashSet<Enemy> Enemies;
    public Tilemap tilemapWalls;
    public Tile wallTile;
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
        if (attacked.Health <= 0 && attacked.IsDead == false)
        {
            Debug.Log(attacked);
            Debug.Log(attacker);    
            ExperienceSystem.Instance.CalculateExperience(attacker, attacked);
            attacked.IsDead = true;
            attacked.Dead();
            Statistics.MobsKilled++;
        }
    }

    private void FixedUpdate()
    {
        var tilePlayer = tilemapWalls.WorldToCell(Player.Instance.transform.position) - tilemapWalls.origin;
        UpdateMap(Enemies);
        foreach (var enemy in Enemies.Where(e => e.CanMove))
        {
            if (enemy.TryGetComponent<MoveAI>(out var moveComponent))
            {
                var enemyPosition = enemyPositions[enemy];
                moveComponent.UpdateAI(map, Player.Instance, (enemyPosition.x, enemyPosition.y), (tilePlayer.x, tilePlayer.y));
            }
        }
    }

    public void InitMap()
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

    public void RemoveGateFromTilemap(Gate gate)
    {
        EditTilemap(gate, null);
    }

    public void AddGateToTilemap(Gate gate)
    {
        EditTilemap(gate, wallTile);
    }

    private void EditTilemap(Gate gate, Tile tile)
    {
        var tilePos = tilemapWalls.WorldToCell(gate.transform.position);
        tilemapWalls.SetTile(tilePos, tile);
        switch (gate.typeGate)
        {
            case TypeGate.Horizontal:
                tilemapWalls.SetTile(tilePos + Vector3Int.right, tile);
                break;
            case TypeGate.Vertical:
                tilemapWalls.SetTile(tilePos + Vector3Int.down, tile);
                break;
        }
        InitMap();
    }
}


public enum GameField
{
    Empty,
    Player,
    Enemy,
    Wall,
}