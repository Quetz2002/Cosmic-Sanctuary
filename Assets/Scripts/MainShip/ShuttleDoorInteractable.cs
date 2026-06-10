using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ShuttleDoorInteractable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 3f;
    public LayerMask playerLayer;

    [Header("Scene Routing")]
    public string shuttleSceneName = "ShuttleScene"; // Name of your intermediate shooter scene

    private Camera playerCamera;
    private TextMeshProUGUI interactionText;
    private bool isPlayerLooking = false;

    private void Start()
    {
        playerCamera = Camera.main;

        // I find the HUD text component automatically from the Player's canvas hierarchy
        PlayerInteraction playerInteract = Object.FindFirstObjectByType<PlayerInteraction>();
        if (playerInteract != null)
        {
            interactionText = playerInteract.interactionText;
        }
    }

    private void Update()
    {
        // I cast a ray from the screen center to see if the player is looking at this door
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            if (hit.collider.gameObject == gameObject)
            {
                // I ensure a planet destination has been selected before letting the player enter the shuttle
                if (GameManager.Instance.currentTargetPlanetIndex != -1)
                {
                    isPlayerLooking = true;
                    if (interactionText != null && !interactionText.gameObject.activeSelf)
                    {
                        interactionText.gameObject.SetActive(true);
                        interactionText.text = "Press F to Enter Shuttle";
                    }

                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        if (interactionText != null) interactionText.gameObject.SetActive(false);

                        // I load the intermediate flight minigame scene
                        SceneManager.LoadScene(shuttleSceneName);
                    }
                }
                else
                {
                    // If no planet is selected, I can show a prompt telling them to use the star map first
                    isPlayerLooking = true;
                    if (interactionText != null && !interactionText.gameObject.activeSelf)
                    {
                        interactionText.gameObject.SetActive(true);
                        interactionText.text = "Set coordinates on the Star Map first!";
                    }
                }
            }
            else
            {
                ClearPrompt();
            }
        }
        else
        {
            ClearPrompt();
        }
    }

    private void ClearPrompt()
    {
        if (isPlayerLooking)
        {
            isPlayerLooking = false;
            if (interactionText != null) interactionText.gameObject.SetActive(false);
        }
    }
}