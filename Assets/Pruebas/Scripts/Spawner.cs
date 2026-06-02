using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;

    [SerializeField] private int spawnAmount;
    [SerializeField] private float minDistance;
    [SerializeField] private int maxAttempts;

    public LayerMask terrain;

    private Collider spawnArea;
    private List<Vector3> spawnPositions = new List<Vector3>();

    void Awake()
    {
        spawnArea = GetComponent<Collider>();
    }

    void Start()
    {
        SpawnPrefabs();
    }

    public void SpawnPrefabs()
    {
        spawnPositions.Clear();
        Bounds bounds = spawnArea.bounds;

        for (int i = 0; i < spawnAmount; i++)
        {
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                Vector3 candidatePos = new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.max.y + 5f, Random.Range(bounds.min.z, bounds.max.z));

                if (!Physics.Raycast(candidatePos, Vector3.down, out RaycastHit hit, Mathf.Infinity, terrain))
                    continue;

                if (IsTooClose(hit.point))
                    continue;

                Instantiate(prefab, hit.point, Random.rotation);
                spawnPositions.Add(hit.point);
                break;
            }
        }
    }

    private bool IsTooClose(Vector3 candidate)
    {
        foreach (Vector3 pos in spawnPositions)
        {
            if (Vector3.Distance(candidate, pos) < minDistance)
                return true;
        }
        return false;
    }
}
