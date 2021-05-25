using System.Collections.Generic;
using PlayerScripts;
using UnityEngine;


public class CheckpointSystem : MonoBehaviour
{
    public Transform lastCheckPoint;

    private readonly HashSet<Transform> checkpoints = new HashSet<Transform>();

    public void Checkpoint(Transform checkpoint)
    {
        if (checkpoints.Add(checkpoint))
        {
            lastCheckPoint = checkpoint;
            Player.Instance.playerSave.SaveData();
            UISystem.Instance.ShowLoadIcon();
        }
    }

    public void LoadLastCheckpoint()
    {
        var player = Player.Instance;
        player.transform.position = lastCheckPoint.position;
        player.playerSave.LoadCheckpoint();
        player.Revival();
        UISystem.Instance.ShowLoadIcon();
    }
}