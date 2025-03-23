using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [Header("波次数据")]
    public int currentWaveCount;
    public float maxEnemiesAllowed;
    public bool reachedMaxEnemies;
    public float enemiesAlive;
    public float enemiesDead;
    public float waveInterval;
    public float spawnerTimer;
    public float waveDuration;
    public float waveTimer;

    [Header("敌人生成")]
    public List<Transform> enemySpawnBounds;

    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public int spawnCount;  //the number of enemies that had been spawned
        public int waveQuota;   //The total number of enemy to spawn in this wave
        public float spawnInterval;
        public List<Transform> placeToSpawn;
        public List<EnemyGroup> enemyGroups;    //A List of enemies to spawn in this wave
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount;  //The number of enemies to spawn in this wave
        public int spawnCount;  //The number of this type already spawned in this wave
        public GameObject enemyPrefab;
    }

    public List<Wave> waves; //A List of all the waves in the game
    bool inWave;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    private void Start()
    {
        CalculateWaveQuota();
    }

    private void Update()
    {
        //PassWaveInfo();
        WaveTimeCounter();

        if (inWave)
        {
            spawnerTimer += Time.deltaTime;
            if (spawnerTimer > waves[currentWaveCount].spawnInterval)
            {
                spawnerTimer = 0;
                SpawnEnemies();
            }
        }
    }

    //private void PassWaveInfo()
    //{
    //    GameManager.instance.currentWave = currentWaveCount;
    //}

    private void WaveTimeCounter()
    {
        waveTimer += Time.deltaTime;
        if (waveTimer >= waveDuration)
        {
            OnBeginNextWave(0f);
        }
    }

    public void OnBeginNextWave(float interval)
    {
        StartCoroutine(BeginNextWave(interval));
    }

    private IEnumerator BeginNextWave(float interval)
    {
        yield return new WaitForSeconds(interval);

        //nextWaveEvent.RaiseEvent();
        inWave = true;
        currentWaveCount++;
        //GameManager.instance.alivedEnemies.Clear();
        CalculateWaveQuota();
    }

    #region 敌人生成

    private void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;

        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;

        waveTimer = 0;
        enemiesDead = 0;
    }

    private void SpawnEnemies()
    {
        //Check if the minimum of enemies in the wave have been spawned
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !reachedMaxEnemies)
        {
            //Spawn each type of enemy until the quota is filled
            foreach (var enemyGroup in waves[(currentWaveCount)].enemyGroups)
            {
                //Check if the minimum number of enemies of this type have been spawned
                if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    //Limit the number of enemies alive
                    if (enemiesAlive >= maxEnemiesAllowed)
                    {
                        reachedMaxEnemies = true;
                        return;
                    }
                    Transform spawnPlace = waves[(currentWaveCount)].placeToSpawn[Random.Range(0, waves[(currentWaveCount)].placeToSpawn.Count)];
                    Bounds spawnBound = new Bounds(spawnPlace.position, spawnPlace.localScale);
                    float x = Random.Range(spawnBound.min.x, spawnBound.max.x);
                    float y = Random.Range(spawnBound.min.y, spawnBound.max.y);
                    Vector2 spawnPosition = new Vector2(x, y);
                    GameObject newEnemy = Instantiate(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity);

                    //GameManager.instance.alivedEnemies.Add(newEnemy);
                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;
                }
            }
        }

        //Reset the reachedMaxEnemies
        if (enemiesAlive < maxEnemiesAllowed)
        {
            reachedMaxEnemies = false;
        }
    }

    public void EnemiesKilled()
    {
        --enemiesAlive;
        ++enemiesDead;
    }

    #endregion

}
