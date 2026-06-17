using UnityEngine;

public class ShuttleShelf : MonoBehaviour
{
    public Transform[] displaySlots; // The exact spots on the shelf where miniatures can sit

    private void Start()
    {
        // I loop through what we've unlocked globally
        for (int i = 0; i < GameManager.Instance.unlockedRewards.Count; i++)
        {
            // I prevent out of bounds if we have more rewards than shelf slots
            if (i >= displaySlots.Length) break;

            RewardItem item = GameManager.Instance.unlockedRewards[i];

            if (item.miniaturePrefab != null)
            {
                // I spawn the miniature on the corresponding slot
                Instantiate(item.miniaturePrefab, displaySlots[i].position, displaySlots[i].rotation, displaySlots[i]);
            }
        }
    }
}