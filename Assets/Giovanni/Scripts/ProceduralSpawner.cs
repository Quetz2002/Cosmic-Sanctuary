using UnityEngine;
using System.Collections.Generic;
using System;

namespace Giovanni.Gameplay
{
    public class ProceduralSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        public GameObject[] prefabs;
        public int spawnCount = 12;
        public BoxCollider spawnVolume;
        public float minCollisionDistance = 1.5f;

        [Header("Ground Alignment")]
        public LayerMask groundLayer;
        public float raycastHeightOffset = 10f;
        public float maxRaycastDistance = 40f;

        // Events
        public event Action<int> OnRemainingCountChanged;
        public event Action OnAllCollected;

        private List<GameObject> activePool = new List<GameObject>();
        private List<Vector3> placedPositions = new List<Vector3>();
        private int totalSpawned = 0;

        private void Start()
        {
            if (spawnVolume == null)
            {
                spawnVolume = GetComponent<BoxCollider>();
            }

            GenerateItems();
        }

        private void GenerateItems()
        {
            if (prefabs == null || prefabs.Length == 0 || spawnVolume == null)
            {
                Debug.LogWarning("Spawner configuration is missing prefabs or volume references.");
                return;
            }

            int successfullySpawned = 0;
            int maxAttempts = spawnCount * 12;
            int attempts = 0;

            Bounds bounds = spawnVolume.bounds;

            while (successfullySpawned < spawnCount && attempts < maxAttempts)
            {
                attempts++;

                // Pick random 2D coordinate inside bounds
                float x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
                float z = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);

                // Shoot raycast downwards from above
                Vector3 origin = new Vector3(x, bounds.max.y + raycastHeightOffset, z);
                Ray ray = new Ray(origin, Vector3.down);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, maxRaycastDistance + raycastHeightOffset, groundLayer))
                {
                    Vector3 targetPos = hit.point;

                    // Verify distance from previously placed objects
                    if (!IsPositionTooClose(targetPos))
                    {
                        GameObject selectedPrefab = prefabs[UnityEngine.Random.Range(0, prefabs.Length)];
                        GameObject spawnedObj = Instantiate(selectedPrefab, targetPos, Quaternion.identity, transform);

                        // Ensure proper components are attached dynamically to make setup foolproof
                        EnsureComponentsAttached(spawnedObj);

                        activePool.Add(spawnedObj);
                        placedPositions.Add(targetPos);
                        successfullySpawned++;
                    }
                }
            }

            totalSpawned = successfullySpawned;
            OnRemainingCountChanged?.Invoke(activePool.Count);
        }

        private bool IsPositionTooClose(Vector3 pos)
        {
            foreach (Vector3 activePos in placedPositions)
            {
                if (Vector3.Distance(pos, activePos) < minCollisionDistance)
                {
                    return true;
                }
            }
            return false;
        }

        private void EnsureComponentsAttached(GameObject obj)
        {
            // CollectableItem is required
            if (obj.GetComponent<CollectableItem>() == null)
            {
                obj.AddComponent<CollectableItem>();
            }

            // JuicyAbsorb is required for the fly animation
            if (obj.GetComponent<JuicyAbsorb>() == null)
            {
                obj.AddComponent<JuicyAbsorb>();
            }
        }

        /// <summary>
        /// Call this method to report that an item has been collected.
        /// </summary>
        public void UnregisterItem(GameObject obj)
        {
            if (activePool.Contains(obj))
            {
                activePool.Remove(obj);
                OnRemainingCountChanged?.Invoke(activePool.Count);

                if (activePool.Count == 0)
                {
                    OnAllCollected?.Invoke();
                }
            }
        }

        public int GetRemainingCount() => activePool.Count;
        public int GetTotalSpawned() => totalSpawned;
    }
}
