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

    [Header("Door Animation / Movement")]
    public Animator doorAnimator;
    public string openParameterName = "Open";
    public Transform[] slidingDoors;
    public Vector3[] openOffsets; // Local offsets relative to starting positions (e.g., Vector3(1.5f, 0, 0) for sliding right)
    public float openSpeed = 2f;

    private Camera playerCamera;
    private TextMeshProUGUI interactionText;
    private bool isPlayerLooking = false;
    private Vector3[] startPositions;
    private bool doorOpened = false;

    private void Start()
    {
        playerCamera = Camera.main;

        // I find the HUD text component automatically from the Player's canvas hierarchy
        PlayerInteraction playerInteract = Object.FindFirstObjectByType<PlayerInteraction>();
        if (playerInteract != null)
        {
            interactionText = playerInteract.interactionText;
        }

        // Cache the starting local positions of the door panels
        if (slidingDoors != null && slidingDoors.Length > 0)
        {
            startPositions = new Vector3[slidingDoors.Length];
            for (int i = 0; i < slidingDoors.Length; i++)
            {
                if (slidingDoors[i] != null)
                {
                    startPositions[i] = slidingDoors[i].localPosition;
                }
            }
        }
    }

    private void Update()
    {
        // We evaluate if a travel coordinate has been set on the Star Map
        bool hasTarget = GameManager.Instance != null && GameManager.Instance.currentTargetPlanetIndex != -1;

        if (hasTarget && !doorOpened)
        {
            OpenDoor();
        }
        else if (!hasTarget && doorOpened)
        {
            CloseDoor();
        }

        // I cast a ray from the screen center to see if the player is looking at this door
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            if (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform))
            {
                // I ensure a planet destination has been selected before letting the player enter the shuttle
                if (hasTarget)
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

    private void OpenDoor()
    {
        doorOpened = true;

        if (doorAnimator != null)
        {
            doorAnimator.SetBool(openParameterName, true);
        }

        if (slidingDoors != null && slidingDoors.Length > 0 && openOffsets != null && openOffsets.Length > 0)
        {
            StopAllCoroutines();
            StartCoroutine(MoveDoors(true));
        }
    }

    private void CloseDoor()
    {
        doorOpened = false;

        if (doorAnimator != null)
        {
            doorAnimator.SetBool(openParameterName, false);
        }

        if (slidingDoors != null && slidingDoors.Length > 0)
        {
            StopAllCoroutines();
            StartCoroutine(MoveDoors(false));
        }
    }

    private System.Collections.IEnumerator MoveDoors(bool open)
    {
        Vector3[] targetPositions = new Vector3[slidingDoors.Length];

        for (int i = 0; i < slidingDoors.Length; i++)
        {
            if (slidingDoors[i] == null) continue;
            Vector3 offset = (openOffsets != null && i < openOffsets.Length) ? openOffsets[i] : Vector3.zero;
            targetPositions[i] = open ? (startPositions[i] + offset) : startPositions[i];
        }

        bool moving = true;
        while (moving)
        {
            moving = false;
            for (int i = 0; i < slidingDoors.Length; i++)
            {
                if (slidingDoors[i] == null) continue;
                Vector3 current = slidingDoors[i].localPosition;
                Vector3 target = targetPositions[i];

                if (Vector3.Distance(current, target) > 0.005f)
                {
                    slidingDoors[i].localPosition = Vector3.MoveTowards(current, target, openSpeed * Time.deltaTime);
                    moving = true;
                }
                else
                {
                    slidingDoors[i].localPosition = target;
                }
            }
            yield return null;
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