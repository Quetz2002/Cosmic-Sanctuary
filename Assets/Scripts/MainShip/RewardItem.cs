using UnityEngine;

// I define an enum to categorize where this specific item is allowed to be placed
public enum PlacementType { Floor, Wall }

[CreateAssetMenu(fileName = "New Reward", menuName = "Cosmic Sanctuary/Reward Item")]
public class RewardItem : ScriptableObject
{
    public string itemID;
    public string itemName;
    public Sprite uiIcon;

    [Header("Placement Rules")]
    public PlacementType placementType = PlacementType.Floor; // Default is floor

    [Header("Prefabs")]
    public GameObject placementPrefab;
    public GameObject miniaturePrefab;

    [Header("Collision & Physics")]
    public Vector3 extents = new Vector3(0.25f, 0.25f, 0.25f);

    // I added this so we can accurately tell the physics engine where the center of the mesh is relative to the base/back pivot
    public Vector3 centerOffset = new Vector3(0f, 0.25f, 0f);
}