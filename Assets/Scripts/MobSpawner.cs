using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    public Transform[] spawnPositions;
    public GameObject[] mobPrefabs;
    public float timeSpawn = 5f;
    public int maxMobCount = 3;
    public int levelMob;
    
    private int mobCount;
    private float lastTimeSpawn;

    private void LateUpdate()
    {
        if (mobCount < maxMobCount && lastTimeSpawn + timeSpawn < Time.time)
        {
            lastTimeSpawn = Time.time;
            SpawnMob();
        }
    }

    private void SpawnMob()
    {
        var mob = mobPrefabs[Random.Range(0, mobPrefabs.Length)];
        var spawn = spawnPositions[Random.Range(0, spawnPositions.Length)];
        var enemy = Instantiate(mob, spawn.position, Quaternion.identity).GetComponent<Enemy>();
        enemy.level = levelMob;
        enemy.ONDead += () => mobCount--;
        GameManager.Instance.Enemies.Add(enemy);
    }
}
