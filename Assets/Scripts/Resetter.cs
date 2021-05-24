using PlayerScripts;
using ScriptableObjects;
using UnityEngine;

public class Resetter : MonoBehaviour
{
    public PlayerSave playerSave;
    public LevelManager levelManager;

    public void Reset()
    {
        playerSave.ClearData();
        levelManager.ClearData();
    }
}