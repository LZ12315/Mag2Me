using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnGroup
{
    public string name;
    public float spawnInterval = 1;
    public int spawnNum = 1;
    public List<GameObject> objects = new List<GameObject>();

    private float spawnCounter = 0;
    public void SpawnCount(ObjectSpawnManager manager)
    {
        spawnCounter += Time.deltaTime;
        if(spawnCounter >= spawnInterval)
        {
            for(int i = 0; i < spawnNum; i++)
            {
                int index = UnityEngine.Random.Range(0, objects.Count - 1);
                GameObject.Instantiate(objects[index], manager.GetSpawnPlace(), Quaternion.identity);
            }
            spawnCounter = 0;
        }
    }
}

public class ObjectSpawnManager : MonoBehaviour
{
    [SerializeField] private float minSpawnRadius = 3f;
    [SerializeField] private float maxSpawnRadius = 6f;

    [SerializeField] private List<SpawnGroup> spawnGroups = new List<SpawnGroup>();

    bool isSpawn;
    Transform playerTrans;

    private void Start()
    {
        EventCenter.Instance.AddEventListener(EventName.StartSpawn.ToString(), StartSpawn);
        playerTrans = GameObject.FindWithTag("Player").transform;

        StartSpawn();
    }

    private void Update()
    {
        if (!isSpawn || spawnGroups.Count == 0) return;

        foreach(SpawnGroup group in spawnGroups)
            group.SpawnCount(this);
    }

    void StartSpawn()
    {
        isSpawn = true;
    }

    public Vector2 GetSpawnPlace()
    {
        if (playerTrans == null) return Vector2.zero;

        float spawnRadius = UnityEngine.Random.Range(minSpawnRadius, maxSpawnRadius);
        float spawnAngle = UnityEngine.Random.Range(0, 360);
        float radians = spawnAngle * Mathf.Deg2Rad;
        float x = spawnRadius * Mathf.Cos(radians);
        float y = spawnRadius * Mathf.Sin(radians);

        return (Vector2)playerTrans.position + new Vector2(x, y);
    }

}
