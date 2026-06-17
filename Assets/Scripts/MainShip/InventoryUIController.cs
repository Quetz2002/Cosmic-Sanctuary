using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Transform contentParent; // Drag the "Content" object of the ScrollView here
    public GameObject itemButtonPrefab; // Drag your Button Prefab here
    public PlacementSystem placementSystem;

    private bool isMenuOpen = false;

    private void Start()
    {
        inventoryPanel.SetActive(false);
    }

    private void Update()
    {
        // I use Tab to open and close the scrollable inventory
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isMenuOpen = !isMenuOpen;
        inventoryPanel.SetActive(isMenuOpen);

        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.isEditingUI = isMenuOpen;
            Cursor.lockState = isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isMenuOpen;
        }

        if (isMenuOpen) PopulateGrid();
    }

    private void PopulateGrid()
    {
        // I clean the existing buttons
        foreach (Transform child in contentParent) Destroy(child.gameObject);

        // I loop through all unlocked rewards in the GameManager
        foreach (RewardItem item in GameManager.Instance.unlockedRewards)
        {
            // I only create a button if the item is NOT currently placed
            if (!GameManager.Instance.IsItemAlreadyPlaced(item.itemID))
            {
                GameObject newBtnObj = Instantiate(itemButtonPrefab, contentParent);
                Button btn = newBtnObj.GetComponent<Button>();

                // FIXED: I grab the image directly from the button itself to prevent crashes
                // regardless of what children the button has.
                Image icon = newBtnObj.GetComponent<Image>();

                if (icon != null && item.uiIcon != null)
                {
                    icon.sprite = item.uiIcon;
                }

                // I add a listener to trigger placement and close menu
                btn.onClick.AddListener(() =>
                {
                    placementSystem.StartPlacingItem(item);
                    ToggleInventory();
                });
            }
        }
    }
}