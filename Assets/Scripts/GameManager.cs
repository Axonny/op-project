using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class GameManager : Singleton<GameManager>
{
    public InputSystem inputSystem;
    public QuestSystem questSystem;
    public ExperienceSystem experienceSystem;
    public UISystem uiSystem;

    private GameField[,] map;
    public List<Enemy> enemies;
    public Tilemap tilemapWalls;
    public Settings settings;

    private void Awake()
    {
        if (settings == null)
            settings = ScriptableObject.CreateInstance<Settings>();
        var size = tilemapWalls.size;
        settings.widthMap = size.x;
        settings.heightMap = size.y;
        map = new GameField[settings.widthMap, settings.heightMap];
        enemies = GameObject.FindObjectsOfType<Enemy>().ToList();
        InitMap();
    }
    
    public void ProceedEnemyDeath(IEnemy enemy, IUnit player) 
    {
     //   experienceSystem.
    }
    public void ProceedDamage(IUnit attacker, IUnit attacked, Damage damage)
    {
        attacked.GetDamage(damage, attacker);
        if (attacked.Health <= 0)
        {
            Debug.Log(attacked);
            Debug.Log(attacker);
            ExperienceSystem.Instance.CalculateExperience(attacker, attacked);
            attacked.Dead();
        }
    }

    private void FixedUpdate()
    {
        UpdateMap(enemies);
        foreach (var enemy in enemies)
        {
            enemy.GetComponent<MoveAI>().UpdateAI(map, Player.Instance);
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

        UpdateMap(enemies);
        PrintMap();
    }

    public void UpdateMap(IEnumerable<Enemy> enemies)
    {
        var tilePlayer = tilemapWalls.WorldToCell(Player.Instance.transform.position) - tilemapWalls.origin;
        map[tilePlayer.x, tilePlayer.y] = GameField.Player;

        foreach (var enemy in enemies)
        {
            var tilePos = tilemapWalls.WorldToCell(enemy.transform.position) - tilemapWalls.origin;
            map[tilePos.x, tilePos.y] = GameField.Enemy;
        }
        
        // PrintMap();
    }

    private void PrintMap()
    {
        var str = new StringBuilder();
        for (var col = 0; col < settings.widthMap; col++)
        {
            for (var row = 0; row < settings.heightMap; row++)
            {
                var i = (int) map[col, row];
                str.Append(i == 0 ? "_" : $"{i}");
            }

            str.Append('\n');
        }
        Debug.Log(str);
    }
}


public enum GameField
{
    Empty,
    Player,
    Enemy,
    Wall,
}