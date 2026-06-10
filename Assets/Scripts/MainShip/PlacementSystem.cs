using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [Header("Placement Settings")]
    public float gridSize = 1f;
    public float placementRange = 5f;
    public LayerMask surfaceLayer; // Only floor, tables, shelves
    public LayerMask obstacleLayer; // Chairs, control panels, other placed items

    [Header("Materials")]
    public Material validMaterial;
    public Material invalidMaterial;

    private Camera playerCamera;
    private GameObject currentPreview;
    private RewardItem currentItemData;
    private bool isValidPlacement;
    private float currentYRotation = 0f;

    // I track if we are currently holding something to place or move
    private bool isPlacingMode = false;

    private void Start()
    {
        playerCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) // Key to toggle build mode for testing
        {
            isPlacingMode = !isPlacingMode;
            if (!isPlacingMode && currentPreview != null) Destroy(currentPreview);
        }

        if (!isPlacingMode)
        {
            CheckForMovingExistingObject();
            return;
        }

        if (currentPreview != null)
        {
            HandlePlacementPreview();
            HandleRotation();
            HandleClickToPlace();
        }
    }

    public void StartPlacingItem(RewardItem item)
    {
        // I destroy the old preview if it exists and spawn a new transparent one
        if (currentPreview != null) Destroy(currentPreview);

        currentItemData = item;
        currentPreview = Instantiate(item.placementPrefab);

        // I disable colliders on the preview so it doesn't interfere with raycasts
        foreach (Collider col in currentPreview.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        isPlacingMode = true;
    }

    private void HandlePlacementPreview()
    {
        // I shoot a raycast from the center of the screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // I combine both layers so the raycast gets physically blocked by obstacles instead of passing through them
        LayerMask combinedMask = surfaceLayer | obstacleLayer;

        if (Physics.Raycast(ray, out RaycastHit hit, placementRange, combinedMask))
        {
            currentPreview.SetActive(true);

            // I calculate the snapped coordinates for the grid
            float snappedX = Mathf.Round(hit.point.x / gridSize) * gridSize;
            float snappedZ = Mathf.Round(hit.point.z / gridSize) * gridSize;

            // I check if the object we hit belongs to the obstacle layer (NotPlaceableSurface)
            bool hitObstacle = ((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0;

            // I check if the surface normal is facing upwards (flat surface like floors or tables)
            bool isFlatSurface = Vector3.Dot(hit.normal, Vector3.up) > 0.9f;

            if (isFlatSurface && !hitObstacle)
            {
                // If it is a valid flat surface and not an obstacle, I align it to the grid perfectly
                Vector3 finalPos = new Vector3(snappedX, hit.point.y, snappedZ);
                currentPreview.transform.position = finalPos;
                currentPreview.transform.rotation = Quaternion.Euler(0, currentYRotation, 0);

                // I run the OverlapBox check to ensure no other placed items are in the way
                CheckIfValid(finalPos);
            }
            else
            {
                // If we look at a wall or an obstacle, I snap the preview directly to the hit impact point
                // This prevents the cube from rendering inside the object and forces it to show as Invalid (Red)
                currentPreview.transform.position = hit.point;
                currentPreview.transform.rotation = Quaternion.Euler(0, currentYRotation, 0);

                isValidPlacement = false;
                ChangePreviewColor(invalidMaterial);
            }
        }
        else
        {
            // I hide the preview if the raycast doesn't hit anything at all
            currentPreview.SetActive(false);
        }
    }

    private void CheckIfValid(Vector3 pos)
    {
        // I use an OverlapBox to ensure no chairs or existing objects are in the way
        bool hittingObstacles = Physics.CheckBox(
            pos + (Vector3.up * currentItemData.extents.y), // Center offset
            currentItemData.extents,
            currentPreview.transform.rotation,
            obstacleLayer
        );

        isValidPlacement = !hittingObstacles;
        ChangePreviewColor(isValidPlacement ? validMaterial : invalidMaterial);
    }

    private void ChangePreviewColor(Material mat)
    {
        // I apply the translucent green or red material to all child renderers
        MeshRenderer[] renderers = currentPreview.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
        {
            Material[] newMats = new Material[r.materials.Length];
            for (int i = 0; i < newMats.Length; i++) newMats[i] = mat;
            r.materials = newMats;
        }
    }

    private void HandleRotation()
    {
        // I allow the player to rotate the object by 90 degrees using R
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentYRotation += 90f;
            if (currentYRotation >= 360f) currentYRotation = 0f;
        }
    }

    // REEMPLAZA EL MÉTODO HandleClickToPlace() COMPLETO POR ESTE:
    private void HandleClickToPlace()
    {
        // I finalize the placement when the user clicks LMB
        if (Input.GetMouseButtonDown(0) && isValidPlacement && currentPreview.activeSelf)
        {
            GameObject placedObject = Instantiate(currentItemData.placementPrefab, currentPreview.transform.position, currentPreview.transform.rotation);

            // FIXED: I changed 'Obstacles' to your actual layer name 'NotPlaceableSurface'
            int targetLayer = LayerMask.NameToLayer("NotPlaceableSurface");
            if (targetLayer != -1)
            {
                SetLayerRecursively(placedObject, targetLayer);
            }
            else
            {
                Debug.LogError("Layer 'NotPlaceableSurface' not found! Check spelling.");
            }

            // I set up the ID but I REMOVED the line that forced the color to white
            PlacedRewardBehavior behavior = placedObject.GetComponent<PlacedRewardBehavior>();
            if (behavior != null)
            {
                behavior.rewardID = currentItemData.itemID;
            }

            // FIXED: I pass a dummy color and -1 emission to tell the GameManager "this is the original prefab material, don't override it"
            GameManager.Instance.SavePlacedItem(currentItemData.itemID, placedObject.transform.position, placedObject.transform.rotation, Color.clear, -1f);

            Destroy(currentPreview);
            isPlacingMode = false;
        }
    }

    // REEMPLAZA EL MÉTODO CheckForMovingExistingObject() COMPLETO POR ESTE:
    private void CheckForMovingExistingObject()
    {
        if (Input.GetMouseButtonDown(1)) // Right click to pick up
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, placementRange, obstacleLayer))
            {
                PlacedRewardBehavior behavior = hit.collider.GetComponentInParent<PlacedRewardBehavior>();

                if (behavior != null)
                {
                    // I tell the GameManager to forget this item was placed, making it available in UI again
                    GameManager.Instance.RemovePlacedItem(behavior.rewardID);
                    Destroy(behavior.gameObject);
                }
            }
        }
    }

    // NEW METHOD: Helper to apply a layer to an object and all its child components
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}