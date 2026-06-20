using UnityEngine;

public class PlacedRewardBehavior : MonoBehaviour
{
    // I store the ID to know which item this is when picking it up or customizing it
    public string rewardID;

    private Renderer itemRenderer;
    private Material instantiatedMaterial;

    private void Awake()
    {
        // I grab the renderer from the child mesh to apply materials
        itemRenderer = GetComponentInChildren<Renderer>();
    }

    public void ApplyCustomization(Color baseColor, float emissionIntensity)
    {
        if (itemRenderer != null)
        {
            // I create a new material instance so changing this item doesn't affect others
            if (instantiatedMaterial == null)
            {
                instantiatedMaterial = itemRenderer.material;
            }
            instantiatedMaterial.color = baseColor;

            // I enable URP emission and apply the intensity
            instantiatedMaterial.EnableKeyword("_EMISSION");
            instantiatedMaterial.SetColor("_EmissionColor", baseColor * emissionIntensity);
        }
    }

    private void OnDestroy()
    {
        if (instantiatedMaterial != null)
        {
            Destroy(instantiatedMaterial);
        }
    }
}