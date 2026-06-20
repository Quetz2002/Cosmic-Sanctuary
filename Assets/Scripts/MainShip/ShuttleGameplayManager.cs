using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ShuttleGameplayManager : MonoBehaviour
{
    [Header("Flight Timing")]
    public float voyageDuration = 20f; // Total time of the shooter sequence

    [Header("Planet Scene Mapping")]
    // I match the planet indices to their respective scene build names
    public string[] planetSceneNames = new string[] { "Planet_Aurora", "Planet_Cryon", "Planet_Ignis", "Planet_Mars", "Pruebas" };

    [Header("Debug/Testing")]
    [Tooltip("If GameManager is not active or target index is -1, load this planet index instead of returning to main ship. Set to -1 to disable.")]
    public int defaultPlanetIndex = -1;

    private bool voyageFinished = false;

    private void Start()
    {
        // I unlock the player controls but ensure mouse capture defaults back to FPS mode
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(StartVoyageCountdown());
    }

    private IEnumerator StartVoyageCountdown()
    {
        yield return new WaitForSeconds(voyageDuration);
        EndVoyage();
    }

    private void EndVoyage()
    {
        if (voyageFinished) return;
        voyageFinished = true;

        string originScene = "";
        if (GameManager.Instance != null)
        {
            originScene = GameManager.Instance.previousSceneName;
        }

        string destinationScene = "";

        if (originScene == "MainShip")
        {
            destinationScene = "Pruebas";
            Debug.Log("[ShuttleGameplayManager] Traveling from MainShip. Destination set to Pruebas.");
        }
        else if (originScene == "Pruebas")
        {
            destinationScene = "MainShip";
            Debug.Log("[ShuttleGameplayManager] Returning from Pruebas. Destination set to MainShip.");
        }
        else
        {
            // If we came from another planet or started in Shuttle scene, use standard target index or debug fallback
            int targetIndex = -1;
            if (GameManager.Instance != null)
            {
                targetIndex = GameManager.Instance.currentTargetPlanetIndex;
            }

            if ((targetIndex < 0 || targetIndex >= planetSceneNames.Length) && defaultPlanetIndex >= 0 && defaultPlanetIndex < planetSceneNames.Length)
            {
                targetIndex = defaultPlanetIndex;
                Debug.Log($"[ShuttleGameplayManager] GameManager target index was invalid. Using debug defaultPlanetIndex: {targetIndex} ({planetSceneNames[targetIndex]})");
            }

            if (targetIndex >= 0 && targetIndex < planetSceneNames.Length)
            {
                destinationScene = planetSceneNames[targetIndex];
            }
            else
            {
                destinationScene = "MainShip";
            }
        }

        Debug.Log("Arriving at destination! Loading scene: " + destinationScene);
        SceneManager.LoadScene(destinationScene);
    }
}