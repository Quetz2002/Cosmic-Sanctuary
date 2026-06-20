using UnityEngine;
using UnityEngine.UI;

public class HologramUIController : MonoBehaviour
{
    public Transform[] uiElements; // Array of UI items representing the rewards
    public Color normalColor = Color.white;
    public Color highlightColor = Color.cyan;

    private int currentIndex = 0;
    private bool isUIAccessible = false;

    public PlacementSystem placementSystem;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) ToggleUI();
        if (!isUIAccessible) return;

        // I navigate the list using W and S
        if (Input.GetKeyDown(KeyCode.W)) MoveSelection(-1);
        if (Input.GetKeyDown(KeyCode.S)) MoveSelection(1);

        // I confirm the selection using Spacebar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmSelection();
        }
    }

    private Canvas hologramCanvas;

    private void Start()
    {
        // I grab the Canvas component and hide it at the start of the game
        hologramCanvas = GetComponent<Canvas>();
        hologramCanvas.enabled = false;
    }

    public void ToggleUI()
    {
        // I am toggling the holographic menu when the player interacts
        isUIAccessible = !isUIAccessible;

        // I visually hide or show the Canvas
        if (hologramCanvas != null) hologramCanvas.enabled = isUIAccessible;

        UpdateHighlight();

        // I tell the player controller to freeze camera movement if UI is open
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null) player.isEditingUI = isUIAccessible;
    }

    private void MoveSelection(int direction)
    {
        if (uiElements == null || uiElements.Length == 0) return;

        currentIndex += direction;
        if (currentIndex < 0) currentIndex = uiElements.Length - 1;
        if (currentIndex >= uiElements.Length) currentIndex = 0;

        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        if (uiElements == null) return;

        // I reset all colors, then highlight the currently selected one to give that cozy, polished feel
        for (int i = 0; i < uiElements.Length; i++)
        {
            if (uiElements[i] != null)
            {
                Image img = uiElements[i].GetComponent<Image>();
                if (img != null)
                {
                    img.color = (i == currentIndex) ? highlightColor : normalColor;
                }
            }
        }
    }

    private void ConfirmSelection()
    {
        if (GameManager.Instance == null) return;

        // I changed 'collectedRewards' to 'unlockedRewards' to match the new GameManager structure
        if (GameManager.Instance.unlockedRewards != null && GameManager.Instance.unlockedRewards.Count > currentIndex)
        {
            RewardItem selectedItem = GameManager.Instance.unlockedRewards[currentIndex];
            if (selectedItem != null && placementSystem != null)
            {
                placementSystem.StartPlacingItem(selectedItem);
                ToggleUI(); // Close the UI after picking an item
            }
        }
    }
}