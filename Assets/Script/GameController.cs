using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;

    [SerializeField]
    private Transform[] spawnPoints;

    private float minTimeSpawnDelay = .1f;
    private float maxTimeSpawnDelay = 1f;

    private int enemyCount = 0;
    private int maxEnemy = 4;

    public bool isSpawning = true;

    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    // Update is called once per frame
    IEnumerator SpawnEnemy()
    {
        while (isSpawning)
        {
            float delay = Random.Range(minTimeSpawnDelay, maxTimeSpawnDelay);
            yield return new WaitForSeconds(1f);

            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[spawnIndex];

            Instantiate(enemy, spawnPoint);
            enemyCount++;
        }
    }
}
