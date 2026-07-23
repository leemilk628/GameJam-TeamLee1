using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    private float timer;
    private bool isSpawn = false;
    [SerializeField] private Slider progressBar;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private GameObject enemyPrefab;

    void Awake()
    {
        progressBar.maxValue = 120;
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        progressBar.value = timer;

        if ((int)timer % 3 == 0)
        {
            if (isSpawn) return;
            
            Spawn();
            isSpawn = true;
        }
        else
        {
            isSpawn = false;
        }
    }

    void Spawn()
    {
        Instantiate(enemyPrefab, spawnPoints[Random.Range(0, spawnPoints.Count)].position,  Quaternion.identity);
    }
}
