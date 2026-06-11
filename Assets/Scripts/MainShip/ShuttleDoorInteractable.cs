using UnityEngine;
using UnityEngine.SceneManagement;

public class ShuttleDoorInteractable : BaseInteractable
{
    [Header("Scene Routing")]
    public string shuttleSceneName = "ShuttleScene";

    public override string GetInteractionPrompt()
    {
        // FIXED: I read the global persistent state layer to dynamically change the UI prompt text context
        if (GameManager.Instance != null && GameManager.Instance.currentTargetPlanetIndex != -1)
        {
            // If coordinates are set, I unlock entry clearance to fly out
            return "Enter shuttle and travel to planet";
        }
        else
        {
            // If the navigation computers are blank, I prompt them to lock in a planet destination first
            return "First choose a planet on the map";
        }
    }

    public override void TriggerInteraction()
    {
        // I safeguard scene transitions to prevent the player from flying into broken spatial parameters
        if (GameManager.Instance != null && GameManager.Instance.currentTargetPlanetIndex != -1)
        {
            SceneManager.LoadScene(shuttleSceneName);
        }
    }
}