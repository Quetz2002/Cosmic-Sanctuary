using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HologramMapController : MonoBehaviour
{
    [Header("Hologram UI Animation")]
    public RectTransform hologramPanel;
    public float revealSpeed = 5f;

    [Header("Planet Selection")]
    public Button confirmButton;
    public Image[] planetButtons;
    public Color selectedColor = Color.cyan;
    public Color idleColor = Color.white;

    [Header("Camera Transition")]
    public Transform mapCameraAnchor; // Where the camera goes to look at the map
    public float cameraTransitionSpeed = 3f;

    [HideInInspector] public bool isMapOpen = false; // Public so the player interaction knows

    private int selectedPlanetIndex = -1;
    private float maxPanelHeight;

    private SpaceTravelManager travelManager;
    private Camera playerCamera;

    // I use these to remember exactly where the camera was before moving it
    private Vector3 originalCamLocalPos;
    private Quaternion originalCamLocalRot;
    private Transform playerCameraParent;

    private void Start()
    {
        travelManager = Object.FindFirstObjectByType<SpaceTravelManager>();
        playerCamera = Camera.main;

        maxPanelHeight = hologramPanel.rect.height;
        hologramPanel.sizeDelta = new Vector2(hologramPanel.sizeDelta.x, 0f);
        hologramPanel.gameObject.SetActive(false);

        if (confirmButton != null) confirmButton.interactable = false;
    }

    private void Update()
    {
        // FIX: I only allow closing the map with Escape. 
        // This prevents the "Double Input" bug where pressing F opens and closes the map in the same frame.
        if (isMapOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMap();
        }
    }

    public void OpenMap()
    {
        if (isMapOpen) return;
        isMapOpen = true;
        StopAllCoroutines();

        // Lock player movement
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null) player.isEditingUI = true;

        StartCoroutine(TransitionCameraAndOpenMap());
    }

    public void CloseMap()
    {
        if (!isMapOpen) return;
        isMapOpen = false;
        StopAllCoroutines();

        // Release the cursor immediately so they can't click things while transitioning back
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(CloseMapAndTransitionCameraBack());
    }

    private IEnumerator TransitionCameraAndOpenMap()
    {
        // 1. Save original camera relative position and rotation
        playerCameraParent = playerCamera.transform.parent;
        originalCamLocalPos = playerCamera.transform.localPosition;
        originalCamLocalRot = playerCamera.transform.localRotation;

        // I detach the camera from the player temporarily to move it freely in the world
        playerCamera.transform.SetParent(null);

        float t = 0f;
        Vector3 startPos = playerCamera.transform.position;
        Quaternion startRot = playerCamera.transform.rotation;

        // 2. Smoothly move the camera to the anchor point for a cozy feel
        while (t < 1f)
        {
            t += Time.deltaTime * cameraTransitionSpeed;
            // I use SmoothStep to give it a nice ease-in and ease-out acceleration
            float curve = Mathf.SmoothStep(0f, 1f, t);
            playerCamera.transform.position = Vector3.Lerp(startPos, mapCameraAnchor.position, curve);
            playerCamera.transform.rotation = Quaternion.Slerp(startRot, mapCameraAnchor.rotation, curve);
            yield return null;
        }

        // Snap exactly to target just in case
        playerCamera.transform.position = mapCameraAnchor.position;
        playerCamera.transform.rotation = mapCameraAnchor.rotation;

        // Unlock mouse for UI interaction now that camera is settled
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 3. Open the Hologram UI
        hologramPanel.gameObject.SetActive(true);
        t = 0f;
        float currentHeight = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * revealSpeed;
            float newHeight = Mathf.Lerp(currentHeight, maxPanelHeight, t);
            hologramPanel.sizeDelta = new Vector2(hologramPanel.sizeDelta.x, newHeight);
            yield return null;
        }
        hologramPanel.sizeDelta = new Vector2(hologramPanel.sizeDelta.x, maxPanelHeight);
    }

    private IEnumerator CloseMapAndTransitionCameraBack()
    {
        // 1. Collapse Hologram UI
        float t = 0f;
        float currentHeight = hologramPanel.sizeDelta.y;
        while (t < 1f)
        {
            t += Time.deltaTime * revealSpeed;
            float newHeight = Mathf.Lerp(currentHeight, 0f, t);
            hologramPanel.sizeDelta = new Vector2(hologramPanel.sizeDelta.x, newHeight);
            yield return null;
        }
        hologramPanel.gameObject.SetActive(false);

        // 2. Smoothly move camera back to the player's head
        t = 0f;
        Vector3 startPos = playerCamera.transform.position;
        Quaternion startRot = playerCamera.transform.rotation;

        while (t < 1f)
        {
            t += Time.deltaTime * cameraTransitionSpeed;
            float curve = Mathf.SmoothStep(0f, 1f, t);

            // I constantly calculate the target world position in case the player object shifted
            Vector3 targetWorldPos = playerCameraParent.TransformPoint(originalCamLocalPos);
            Quaternion targetWorldRot = playerCameraParent.rotation * originalCamLocalRot;

            playerCamera.transform.position = Vector3.Lerp(startPos, targetWorldPos, curve);
            playerCamera.transform.rotation = Quaternion.Slerp(startRot, targetWorldRot, curve);
            yield return null;
        }

        // 3. Re-parent the camera back to the player and reset its local values perfectly
        playerCamera.transform.SetParent(playerCameraParent);
        playerCamera.transform.localPosition = originalCamLocalPos;
        playerCamera.transform.localRotation = originalCamLocalRot;

        // Give movement control back to the player
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null) player.isEditingUI = false;
    }

    public void SelectPlanet(int index)
    {
        GameManager.Instance.currentTargetPlanetIndex = index; // Register the target planet globally
        selectedPlanetIndex = index;
        for (int i = 0; i < planetButtons.Length; i++)
        {
            planetButtons[i].color = (i == index) ? selectedColor : idleColor;
        }
        if (confirmButton != null) confirmButton.interactable = true;
    }

    public void ConfirmSelection()
    {
        if (selectedPlanetIndex != -1)
        {
            travelManager.StartTravelAnimation(selectedPlanetIndex);
            CloseMap(); // Close the map gracefully and fly the camera back before traveling
        }
    }
}