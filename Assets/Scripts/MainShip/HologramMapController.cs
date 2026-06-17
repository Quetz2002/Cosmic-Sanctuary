using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HologramMapController : MonoBehaviour
{
    [Header("Hologram Animation")]
    public RectTransform hologramPanel; // The main panel containing the map graphics
    public float revealSpeed = 2f;

    [Header("Selection Settings")]
    public Button confirmButton;
    public Color selectedColor = Color.cyan;
    public Color idleColor = Color.white;
    public Image[] planetSelectionImages; // The 4 buttons visual representation

    private int currentlySelectedPlanet = -1;
    private bool isMenuOpen = false;
    private Vector2 originalSize;

    // I reference the travel manager to trigger the world animation upon confirmation
    private SpaceTravelManager travelManager;

    private void Start()
    {
        travelManager = Object.FindFirstObjectByType<SpaceTravelManager>();

        // I store the target size of the hologram panel and collapse it instantly
        originalSize = hologramPanel.sizeDelta;
        hologramPanel.sizeDelta = new Vector2(originalSize.x, 0f);
        hologramPanel.gameObject.SetActive(false);

        if (confirmButton != null) confirmButton.interactable = false;
    }

    private void Update()
    {
        // Will use M key to test opening/closing the starmap console for now
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap();
        }
    }

    public void ToggleMap()
    {
        isMenuOpen = !isMenuOpen;
        StopAllCoroutines();

        // Free or lock the mouse cursor so the player can interact with the hologram buttons
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.isEditingUI = isMenuOpen;
            Cursor.lockState = isMenuOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isMenuOpen;
        }

        StartCoroutine(AnimateHologram(isMenuOpen));
    }

    private IEnumerator AnimateHologram(bool open)
    {
        if (open) hologramPanel.gameObject.SetActive(true);

        float currentHeight = hologramPanel.sizeDelta.y;
        float targetHeight = open ? originalSize.y : 0f;
        float t = 0f;

        // Smoothly interpolate the vertical height to give that cool "revelation from bottom to top" effect
        while (t < 1f)
        {
            t += Time.deltaTime * revealSpeed;
            float newHeight = Mathf.Lerp(currentHeight, targetHeight, t);
            hologramPanel.sizeDelta = new Vector2(originalSize.x, newHeight);
            yield return null;
        }

        if (!open) hologramPanel.gameObject.SetActive(false);
    }

    public void SelectPlanet(int planetIndex)
    {
        // Handle the selection of one of the 4 planets from the UI buttons
        currentlySelectedPlanet = planetIndex;

        // Update the visual highlights of the buttons to emphasize selection
        for (int i = 0; i < planetSelectionImages.Length; i++)
        {
            planetSelectionImages[i].color = (i == planetIndex) ? selectedColor : idleColor;
        }

        if (confirmButton != null) confirmButton.interactable = true;
    }

    public void ConfirmTravel()
    {
        if (currentlySelectedPlanet != -1)
        {
            // Set the target planet index in the GameManager for the voyage sequence
            if (GameManager.Instance != null)
            {
                GameManager.Instance.currentTargetPlanetIndex = currentlySelectedPlanet;
            }

            // Tell the space travel manager to execute the movement animation outside the window
            travelManager.StartTravelAnimation(currentlySelectedPlanet);

            // Automatically close the map after confirming the destination
            ToggleMap();
        }
    }
}