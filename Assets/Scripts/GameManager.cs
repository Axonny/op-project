using System;
using DefaultNamespace;
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
    public Tilemap tilemapWalls;
    public Settings settings;

    private void Awake()
    {
        map = new GameField[settings.widthMap, settings.heightMap];
        //UpdateMap();
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

    public void UpdateMap()
    {
        for (var col = 0; col < settings.widthMap; col++)
        for (var row = 0; row < settings.heightMap; row++)
        {
            var tileWall = tilemapWalls.GetTile(
                new Vector3Int(col - settings.widthMap / 2, row - settings.heightMap / 2, 0)
            );

            map[col, row] = tileWall is null ? GameField.Empty : GameField.Wall;
        }
    }
}


public enum GameField
{
    Empty,
    Player,
    Enemy,
    Wall,
}