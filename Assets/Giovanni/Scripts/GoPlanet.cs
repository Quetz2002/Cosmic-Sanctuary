using UnityEngine;
using UnityEngine.SceneManagement;

public class GoPlanet : MonoBehaviour
{
    [Header("Scene Routing")]
    public string shuttleSceneName = "Shuttle";
    public string targetSceneName = "Pruebas";

    [Header("Trigger Options")]
    [Tooltip("If true, the transition will start automatically on Start.")]
    public bool autoStart = false;

    [Tooltip("If true, the transition will start when a player enters the trigger zone.")]
    public bool triggerOnCollision = true;

    [Tooltip("The tag of the player GameObject to detect collision.")]
    public string playerTag = "Player";

    private void Start()
    {
        if (autoStart)
        {
            StartTravel();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnCollision && other.CompareTag(playerTag))
        {
            StartTravel();
        }
    }

    [ContextMenu("Start Travel")]
    public void StartTravel()
    {
        if (GameManager.Instance != null)
        {
            int index = GetTargetIndex(targetSceneName);
            GameManager.Instance.currentTargetPlanetIndex = index;
            Debug.Log($"[GoPlanet] Setting target planet index to {index} ({targetSceneName})");
        }
        else
        {
            Debug.LogWarning("[GoPlanet] GameManager.Instance is null. Cannot set currentTargetPlanetIndex, but loading shuttle scene anyway.");
        }

        Debug.Log($"[GoPlanet] Loading shuttle scene: {shuttleSceneName}");
        SceneManager.LoadScene(shuttleSceneName);
    }

    private int GetTargetIndex(string sceneName)
    {
        switch (sceneName)
        {
            case "Planet_Aurora": return 0;
            case "Planet_Cryon": return 1;
            case "Planet_Ignis": return 2;
            case "Planet_Mars": return 3;
            case "Pruebas": return 4;
            case "MainShip":
            case "ShipScene":
                return -1;
            default:
                Debug.LogWarning($"[GoPlanet] Scene '{sceneName}' not in predefined map, defaulting to index 4 (Pruebas)");
                return 4;
        }
    }
}