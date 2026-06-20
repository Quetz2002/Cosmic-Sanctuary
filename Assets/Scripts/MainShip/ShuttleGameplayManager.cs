using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ShuttleGameplayManager : MonoBehaviour
{
    [Header("Flight Timing")]
    public float voyageDuration = 20f; // Total time of the shooter sequence

    [Header("Planet Scene Mapping")]
    // I match the planet indices (0,1,2,3) to their respective scene build names
    public string[] planetSceneNames = new string[] { "Planet_Aurora", "Planet_Cryon", "Planet_Ignis", "Planet_Mars" };

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

        int targetIndex = -1;
        if (GameManager.Instance != null)
        {
            targetIndex = GameManager.Instance.currentTargetPlanetIndex;
        }

        // Safety fallback: if no planet index was saved, I return the player to the Ship
        if (targetIndex < 0 || targetIndex >= planetSceneNames.Length)
        {
            Debug.LogError("Invalid target planet index! Returning to main ship.");
            SceneManager.LoadScene("ShipScene");
        }
        else
        {
            // I fetch the correct scene name and load the planetary exploration zone
            string destinationScene = planetSceneNames[targetIndex];
            Debug.Log("Arriving at destination! Loading scene: " + destinationScene);
            SceneManager.LoadScene(destinationScene);
        }
    }
}