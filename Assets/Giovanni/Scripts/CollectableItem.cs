using UnityEngine;

namespace Giovanni.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class CollectableItem : MonoBehaviour
    {
        [Header("Item Configuration")]
        [Tooltip("The display name of this item.")]
        public string itemName = "Space Mineral";

        [Tooltip("How many points this item is worth when deposited.")]
        public int scoreValue = 1;

        [Header("Effects")]
        [Tooltip("Optional prefab to instantiate when the item is successfully collected.")]
        public GameObject collectEffectPrefab;

        private Collider itemCollider;
        private bool isBeingCollected = false;

        private void Awake()
        {
            itemCollider = GetComponent<Collider>();
        }

        /// <summary>
        /// Initiates the collection phase. Disables collisions so it can fly freely.
        /// </summary>
        public bool InitiateCollection()
        {
            if (isBeingCollected) return false;

            isBeingCollected = true;
            if (itemCollider != null)
            {
                itemCollider.enabled = false;
            }
            return true;
        }

        /// <summary>
        /// Triggers the final collection effects and returns the score value.
        /// </summary>
        public int FinalizeCollection()
        {
            if (collectEffectPrefab != null)
            {
                GameObject fx = Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
                Destroy(fx, 2f);
            }

            return scoreValue;
        }
    }
}
