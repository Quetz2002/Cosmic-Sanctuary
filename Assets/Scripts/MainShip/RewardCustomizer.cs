using UnityEngine;

public class RewardCustomizer : MonoBehaviour
{
    public float interactRange = 5f;
    public LayerMask obstacleLayer; // The layer where placed items live
    public int customizationCost = 5; // Cost in cosmic materials

    // Array of cozy colors to cycle through
    public Color[] cozyColors = new Color[] { Color.white, Color.cyan, Color.magenta, Color.green, Color.yellow };

    private Camera playerCamera;

    private void Start()
    {
        playerCamera = Camera.main;
    }

    private void Update()
    {
        // I use the 'C' key to customize the object I am looking at
        if (Input.GetKeyDown(KeyCode.C))
        {
            TryCustomizeObject();
        }
    }

    private void TryCustomizeObject()
    {
        if (GameManager.Instance.cosmicMaterials < customizationCost)
        {
            Debug.Log("Not enough materials to customize!");
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, obstacleLayer))
        {
            PlacedRewardBehavior behavior = hit.collider.GetComponentInParent<PlacedRewardBehavior>();

            if (behavior != null)
            {
                // I charge the materials
                GameManager.Instance.cosmicMaterials -= customizationCost;

                // I pick a random color from the palette and set a glowing emission
                Color newColor = cozyColors[Random.Range(0, cozyColors.Length)];
                float newEmission = 1.5f; // Glow intensity

                // I apply it visually
                behavior.ApplyCustomization(newColor, newEmission);

                // I find the saved data in GameManager and update it so it survives scene changes!
                PlacedItemData data = GameManager.Instance.placedItemsData.Find(item => item.itemID == behavior.rewardID);
                if (data != null)
                {
                    data.customColor = newColor;
                    data.emissionIntensity = newEmission;
                }
            }
        }
    }
}