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
        hologramCanvas.enabled = isUIAccessible;

        UpdateHighlight();

        // I tell the player controller to freeze camera movement if UI is open
        Object.FindFirstObjectByType<PlayerController>().isEditingUI = isUIAccessible;
    }

    private void MoveSelection(int direction)
    {
        currentIndex += direction;
        if (currentIndex < 0) currentIndex = uiElements.Length - 1;
        if (currentIndex >= uiElements.Length) currentIndex = 0;

        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        // I reset all colors, then highlight the currently selected one to give that cozy, polished feel
        for (int i = 0; i < uiElements.Length; i++)
        {
            uiElements[i].GetComponent<Image>().color = (i == currentIndex) ? highlightColor : normalColor;
        }
    }

    private void ConfirmSelection()
    {
        // I grab the actual reward from the GameManager and pass it to the Placement System
        if (GameManager.Instance.collectedRewards.Count > currentIndex)
        {
            RewardItem selectedItem = GameManager.Instance.collectedRewards[currentIndex];
            placementSystem.StartPlacingItem(selectedItem);
            ToggleUI(); // Close the UI after picking an item
        }
    }
}