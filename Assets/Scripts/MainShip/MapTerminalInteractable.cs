using UnityEngine;

public class MapTerminalInteractable : BaseInteractable
{
    private HologramMapController mapController;

    private void Start()
    {
        mapController = Object.FindFirstObjectByType<HologramMapController>();
    }

    public override string GetInteractionPrompt()
    {
        // I return the classic command prompt text for the holographic star map console
        return "Press F to open Map";
    }

    public override void TriggerInteraction()
    {
        if (mapController != null)
        {
            // I initiate the camera fly-to animation and spawn the holographic star chart UI
            mapController.OpenMap();
        }
    }
}