using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private GameObject enemyPrefab;
    
    public void Spawn(GameObject[] entities)
    {
        if (entities.Length == 1)
        {
            Instantiate(entities[0], spawnPoints[6].position,  Quaternion.identity);
        }
        else
        {
            Instantiate(entities[Random.Range(0, entities.Length)], spawnPoints[Random.Range(0, spawnPoints.Count)].position,  Quaternion.identity);
        }
    }
}
