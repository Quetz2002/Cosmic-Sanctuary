using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 3f;
    public LayerMask interactableLayer; // Layer containing our panels and doors

    [Header("UI References")]
    public GameObject crosshair;
    public TextMeshProUGUI interactionText;

    private Camera playerCamera;
    private HologramMapController mapController;

    private void Start()
    {
        playerCamera = Camera.main;
        if (interactionText != null) interactionText.gameObject.SetActive(false);
        mapController = Object.FindFirstObjectByType<HologramMapController>();
    }

    private void Update()
    {
        // I don't want to scan for world interactions if the star map is currently active
        if (mapController != null && mapController.isMapOpen)
        {
            if (interactionText.gameObject.activeSelf) interactionText.gameObject.SetActive(false);
            if (crosshair != null && crosshair.activeSelf) crosshair.SetActive(false);
            return;
        }

        if (crosshair != null && !crosshair.activeSelf) crosshair.SetActive(true);

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // I cast a single raycast to detect what interactive element sits in front of the player
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            // I check if the hit object inherits from our clean abstract interactable structure
            BaseInteractable interactable = hit.collider.GetComponent<BaseInteractable>();

            if (interactable != null)
            {
                if (!interactionText.gameObject.activeSelf) interactionText.gameObject.SetActive(true);

                // FIXED: I dynamically fetch the context-aware prompt text calculated by the object itself
                interactionText.text = interactable.GetInteractionPrompt();

                // I listen for the interaction input key trigger
                if (Input.GetKeyDown(KeyCode.F))
                {
                    interactable.TriggerInteraction();
                }
            }
        }
        else
        {
            if (interactionText.gameObject.activeSelf) interactionText.gameObject.SetActive(false);
        }
    }
}