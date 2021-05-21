using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


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
        }
    }

    public void LoadLastCheckpoint()
    {
        var player = Player.Instance;
        player.Health = 100;
        player.GetComponent<MagicUnit>().Mana = 100;
        player.transform.position = lastCheckPoint.position;
    }
}