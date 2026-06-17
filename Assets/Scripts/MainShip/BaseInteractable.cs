using UnityEngine;

public abstract class BaseInteractable : MonoBehaviour
{
    // I force all interactable objects to define their own text prompt dynamically
    public abstract string GetInteractionPrompt();

    // I force all interactable objects to define what action happens when F is pressed
    public abstract void TriggerInteraction();
}