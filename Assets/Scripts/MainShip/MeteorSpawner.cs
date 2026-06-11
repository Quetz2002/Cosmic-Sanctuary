using UnityEngine;
using System.Collections.Generic;

public class MeteorSpawner : MonoBehaviour
{
    [Header("Spawning Coordinates")]
    public GameObject[] meteorPrefabs; // Array of different meteor models
    public float spawnInterval = 1.5f;

    [Header("Spawn Box Area")]
    public Vector3 spawnAreaCenter;
    public Vector3 spawnAreaSize;

    private List<GameObject>[] pooledMeteors;

    private void Start()
    {
        // Initialize the array of lists for pooling each meteor prefab type
        pooledMeteors = new List<GameObject>[meteorPrefabs.Length];
        for (int i = 0; i < meteorPrefabs.Length; i++)
        {
            pooledMeteors[i] = new List<GameObject>();
        }

        // I invoke the spawning system repeatedly based on the custom interval rates
        InvokeRepeating(nameof(SpawnRandomMeteor), 0.5f, spawnInterval);
    }

    private void SpawnRandomMeteor()
    {
        if (meteorPrefabs.Length == 0) return;

        // I calculate a random point inside our designated spatial bounds box
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );

        Vector3 spawnPosition = transform.position + spawnAreaCenter + randomOffset;
        Quaternion spawnRotation = Random.rotation;

        int index = Random.Range(0, meteorPrefabs.Length);
        GameObject selectedPrefab = meteorPrefabs[index];

        // Find an inactive object in the corresponding pool
        GameObject meteor = null;
        for (int i = 0; i < pooledMeteors[index].Count; i++)
        {
            if (pooledMeteors[index][i] != null && !pooledMeteors[index][i].activeSelf)
            {
                meteor = pooledMeteors[index][i];
                break;
            }
        }

        if (meteor != null)
        {
            meteor.transform.position = spawnPosition;
            meteor.transform.rotation = spawnRotation;
            meteor.SetActive(true);
        }
        else
        {
            // If no inactive object exists, spawn a new one and add it to the pool
            meteor = Instantiate(selectedPrefab, spawnPosition, spawnRotation);
            pooledMeteors[index].Add(meteor);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // I draw a visual wireframe box in the editor view so I can safely adjust spawn bounds
        Gizmos.color = Color.orange;
        Gizmos.DrawWireCube(transform.position + spawnAreaCenter, spawnAreaSize);
    }
}