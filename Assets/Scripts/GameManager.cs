using System;
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
    
    public void proceedEnemyDeath(IEnemy enemy, IUnit player) 
    {
     //   experienceSystem.
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