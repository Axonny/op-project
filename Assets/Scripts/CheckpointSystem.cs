using System.Collections.Generic;
using UnityEngine;


public class CheckpointSystem : MonoBehaviour
{
    public Transform lastCheckPoint;

    private HashSet<Transform> checkpoints = new HashSet<Transform>();

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
        player.playerSave.LoadData();
        player.Revival();
        UISystem.Instance.ShowLoadIcon();
    }
}