using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlacedItemData
{
    // I store the bare minimum data needed to rebuild an object later
    public string itemID;
    public Vector3 position;
    public Quaternion rotation;
    public Color customColor;
    public float emissionIntensity;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Inventory")]
    public List<RewardItem> unlockedRewards = new List<RewardItem>();
    public int cosmicMaterials = 0; // The resource used for customization

    [Header("Ship State")]
    public List<PlacedItemData> placedItemsData = new List<PlacedItemData>();

    private void Awake()
    {
        // I ensure this manager survives all scene loads
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // I subscribe to the scene load event to rebuild the ship when we return
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Whenever a scene loads, if it's the Ship scene, I rebuild the placed items
        if (scene.name == "ShipScene") // CHANGE THIS TO YOUR ACTUAL SHIP SCENE NAME
        {
            RebuildShipEnvironment();
        }
    }

    public void UnlockReward(RewardItem newReward)
    {
        // I allow planet task scripts to call this method to unlock items
        if (!unlockedRewards.Contains(newReward))
        {
            unlockedRewards.Add(newReward);
            Debug.Log("Unlocked new reward: " + newReward.itemName);
        }
    }

    public void AddMaterial(int amount)
    {
        cosmicMaterials += amount;
    }

    public bool IsItemAlreadyPlaced(string id)
    {
        // I check if an item is currently placed in the ship
        return placedItemsData.Exists(item => item.itemID == id);
    }

    public void SavePlacedItem(string id, Vector3 pos, Quaternion rot, Color col, float emission)
    {
        PlacedItemData newData = new PlacedItemData { itemID = id, position = pos, rotation = rot, customColor = col, emissionIntensity = emission };
        placedItemsData.Add(newData);
    }

    public void RemovePlacedItem(string id)
    {
        placedItemsData.RemoveAll(item => item.itemID == id);
    }

    private void RebuildShipEnvironment()
    {
        // I loop through saved data and spawn the prefabs in their correct locations
        foreach (PlacedItemData data in placedItemsData)
        {
            RewardItem originalItem = unlockedRewards.Find(r => r.itemID == data.itemID);
            if (originalItem != null)
            {
                GameObject spawned = Instantiate(originalItem.placementPrefab, data.position, data.rotation);

                // Assign to obstacles layer
                SetLayerRecursively(spawned, LayerMask.NameToLayer("Obstacles"));

                // Apply customizations
                PlacedRewardBehavior behavior = spawned.GetComponent<PlacedRewardBehavior>();
                if (behavior != null)
                {
                    behavior.rewardID = data.itemID;
                    behavior.ApplyCustomization(data.customColor, data.emissionIntensity);
                }
            }
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (newLayer == -1) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, newLayer);
    }
}