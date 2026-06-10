using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    [Header("Spawning Coordinates")]
    public GameObject[] meteorPrefabs; // Array of different meteor models
    public float spawnInterval = 1.5f;

    [Header("Spawn Box Area")]
    public Vector3 spawnAreaCenter;
    public Vector3 spawnAreaSize;

    private void Start()
    {
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
        GameObject selectedPrefab = meteorPrefabs[Random.Range(0, meteorPrefabs.Length)];

        // I spawn the chosen meteor model into the world
        Instantiate(selectedPrefab, spawnPosition, Random.rotation);
    }

    private void OnDrawGizmosSelected()
    {
        // I draw a visual wireframe box in the editor view so I can safely adjust spawn bounds
        Gizmos.color = Color.orange;
        Gizmos.DrawWireCube(transform.position + spawnAreaCenter, spawnAreaSize);
    }
}