using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyGroup
{
    public GameObject enemyprefab;
    public int spawnNum;
}

[Serializable]
public class Wave
{
    public string waveName;
    public float waveInterval = 0;
    public List<EnemyGroup> enemyGroups = new List<EnemyGroup>();
    [HideInInspector]
    public int enemyNumToSpawn;
    [HideInInspector]
    public int enemyNumSpawned;
    [HideInInspector] 
    public List<GameObject> enemyAlive = new List<GameObject>();
}

public class EnemyWaveManager : MonoBehaviour
{
    [SerializeField] private float enemySpawnInterval = 2;
    [SerializeField] private float minSpawnRadius = 3f;
    [SerializeField] private float maxSpawnRadius = 6f;

    [SerializeField] private Wave currentWave;
    [SerializeField] private List<Wave> waves = new List<Wave>();

    int waveCount = 0;
    Transform playerTrans;

    private void Start()
    {
        EventCenter.Instance.AddEventListener(EventName.StartSpawn.ToString(), StartSpawn);
        EventCenter.Instance.AddEventListener<GameObject>(EventName.EnemyDead.ToString(), (value) => RecycleEnemy(value));
        playerTrans = GameObject.FindWithTag("Player").transform;
        waveCount = 0;

        StartSpawn();
    }

    void StartSpawn()
    {
        if (waves.Count == 0) return;
        WaveStart();
    }

    void WaveStart()
    {
        if (waveCount >= waves.Count) return;

        currentWave = waves[waveCount];

        foreach (var group in currentWave.enemyGroups)
            currentWave.enemyNumToSpawn += group.spawnNum;

        StartCoroutine(SpawnEnemy());
    }

    IEnumerator NextWave()
    {
        if (waveCount+1 >= waves.Count) yield break;

        yield return new WaitForSeconds(currentWave.waveInterval);
        waveCount++;
        WaveStart();
    }

    IEnumerator SpawnEnemy()
    {
        foreach (var group in currentWave.enemyGroups)
        {
            for (int i = 0; i< group.spawnNum; i++)
            {
                GameObject newEnemy = Instantiate(group.enemyprefab, GetSpawnPlace(), Quaternion.identity);

                currentWave.enemyAlive.Add(newEnemy);
                currentWave.enemyNumSpawned++;
                yield return new WaitForSeconds(enemySpawnInterval);
            }
        }
    }

    void RecycleEnemy(GameObject diedEnemy)
    {
        if(currentWave.enemyAlive.Contains(diedEnemy))
        {
            currentWave.enemyAlive.Remove(diedEnemy);
            if (currentWave.enemyNumSpawned == currentWave.enemyNumToSpawn && currentWave.enemyAlive.Count == 0)
                StartCoroutine(NextWave());
        }
    }

    Vector2 GetSpawnPlace()
    {
        if(playerTrans == null) return Vector2.zero;

        float spawnRadius = UnityEngine.Random.Range(minSpawnRadius, maxSpawnRadius);
        float spawnAngle = UnityEngine.Random.Range(0, 360);
        float radians = spawnAngle * Mathf.Deg2Rad;
        float x = spawnRadius * Mathf.Cos(radians);
        float y = spawnRadius * Mathf.Sin(radians);

        return (Vector2)playerTrans.position + new Vector2(x, y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, minSpawnRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, maxSpawnRadius);
    }

}
