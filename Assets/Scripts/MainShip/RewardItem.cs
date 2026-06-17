using UnityEngine;

[CreateAssetMenu(fileName = "New Reward", menuName = "Cosmic Sanctuary/Reward Item")]
public class RewardItem : ScriptableObject
{
    // I am giving each item a unique string ID to track it across scenes
    public string itemID;
    public string itemName;
    public Sprite uiIcon; // Used for the new scrollable inventory

    public GameObject placementPrefab;
    public GameObject miniaturePrefab;

    public Vector3 extents = new Vector3(0.25f, 0.25f, 0.25f);
}