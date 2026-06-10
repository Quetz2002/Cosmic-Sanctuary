using UnityEngine;
using TMPro; // Usaremos TextMeshPro para un texto HD

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 3f;
    public LayerMask interactableLayer; // We will create a layer just for panels/terminals

    [Header("UI References")]
    public GameObject crosshair; // Optional: A small dot in the center of the screen
    public TextMeshProUGUI interactionText; // The "Press F to..." text

    private Camera playerCamera;
    private HologramMapController mapController;

    private void Start()
    {
        playerCamera = Camera.main;

        // I hide the prompt text at the start of the game
        if (interactionText != null) interactionText.gameObject.SetActive(false);

        mapController = Object.FindFirstObjectByType<HologramMapController>();
    }

    private void Update()
    {
        // I don't want to check for interactions if the map is already open
        if (mapController != null && mapController.isMapOpen)
        {
            if (interactionText.gameObject.activeSelf) interactionText.gameObject.SetActive(false);
            if (crosshair != null && crosshair.activeSelf) crosshair.SetActive(false);
            return;
        }

        // Make sure the crosshair is visible when exploring
        if (crosshair != null && !crosshair.activeSelf) crosshair.SetActive(true);

        // I cast a ray from the center of my screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            // If I am looking at the console, I show the prompt
            interactionText.gameObject.SetActive(true);
            interactionText.text = "Press F to open Map";

            // I listen for the interact key
            if (Input.GetKeyDown(KeyCode.F))
            {
                interactionText.gameObject.SetActive(false);
                mapController.OpenMap(); // Tell the map to take over
            }
        }
        else
        {
            // If I look away, hide the prompt
            interactionText.gameObject.SetActive(false);
        }
    }
}