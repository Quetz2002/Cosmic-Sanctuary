using UnityEngine;

[CreateAssetMenu(fileName = "New Reward", menuName = "Cosmic Sanctuary/Reward Item")]
public class RewardItem : ScriptableObject
{
    // I am defining the core data for each reward we find on planets
    public string itemName;
    public GameObject placementPrefab; // The full-size prefab for the Ship
    public GameObject miniaturePrefab; // The small version for Giovanni's Shuttle
    public Vector3 extents = new Vector3(0.5f, 0.5f, 0.5f); // Used for collision checking
}