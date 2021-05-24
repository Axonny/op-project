using PlayerScripts;
using UnityEngine;

public class Resetter : MonoBehaviour
{
    public PlayerSave playerSave;

    public void Reset()
    {
        playerSave.ClearData();
    }
}