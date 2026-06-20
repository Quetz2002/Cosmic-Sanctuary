using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [Header("Placement Settings")]
    public float gridSize = 1f;
    public float placementRange = 5f;
    public LayerMask surfaceLayer; // Floor AND Walls must be on this layer
    public LayerMask obstacleLayer; // Placed items and interactables

    [Header("Materials")]
    public Material validMaterial;
    public Material invalidMaterial;

    private Camera playerCamera;
    private GameObject currentPreview;
    private RewardItem currentItemData;
    private bool isValidPlacement;
    private float currentRotationAngle = 0f;

    private bool isPlacingMode = false;

    private void Start()
    {
        if (playerCamera == null) playerCamera = Camera.main;
    }

    private void Update()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (playerCamera == null) return;

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
        // I prepare the system to start placing a new item selected from the inventory
        if (currentPreview != null) Destroy(currentPreview);

        currentItemData = item;
        currentPreview = Instantiate(item.placementPrefab);

        // I disable colliders so the preview doesn't block its own raycasts
        foreach (Collider col in currentPreview.GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }

        isPlacingMode = true;
        currentRotationAngle = 0f; // Reset rotation for new items
    }

    private void HandlePlacementPreview()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        LayerMask combinedMask = surfaceLayer | obstacleLayer;

        if (Physics.Raycast(ray, out RaycastHit hit, placementRange, combinedMask))
        {
            currentPreview.SetActive(true);
            bool hitObstacle = ((1 << hit.collider.gameObject.layer) & obstacleLayer) != 0;

            // I use normal vectors to mathematically determine if the surface is a floor or a wall
            bool isFlatFloor = Vector3.Dot(hit.normal, Vector3.up) > 0.9f;
            bool isVerticalWall = Mathf.Abs(hit.normal.y) < 0.1f;

            bool isValidSurfaceType = false;
            Vector3 finalPos = hit.point;
            Quaternion finalRot = Quaternion.identity;

            if (!hitObstacle)
            {
                // LOGIC FOR FLOOR ITEMS
                if (currentItemData.placementType == PlacementType.Floor && isFlatFloor)
                {
                    isValidSurfaceType = true;
                    float snappedX = Mathf.Round(hit.point.x / gridSize) * gridSize;
                    float snappedZ = Mathf.Round(hit.point.z / gridSize) * gridSize;

                    finalPos = new Vector3(snappedX, hit.point.y, snappedZ);
                    finalRot = Quaternion.Euler(0, currentRotationAngle, 0);
                }
                // LOGIC FOR WALL ITEMS
                else if (currentItemData.placementType == PlacementType.Wall && isVerticalWall)
                {
                    isValidSurfaceType = true;

                    float snappedX = Mathf.Round(hit.point.x / gridSize) * gridSize;
                    float snappedY = Mathf.Round(hit.point.y / gridSize) * gridSize;
                    float snappedZ = Mathf.Round(hit.point.z / gridSize) * gridSize;

                    // I align the snapping plane depending on which direction the wall is facing
                    if (Mathf.Abs(hit.normal.x) > 0.9f) // Wall goes along Z
                        finalPos = new Vector3(hit.point.x, snappedY, snappedZ);
                    else if (Mathf.Abs(hit.normal.z) > 0.9f) // Wall goes along X
                        finalPos = new Vector3(snappedX, snappedY, hit.point.z);
                    else // Fallback for diagonal walls
                        finalPos = hit.point;

                    // I rotate the object to stick its back flat against the wall
                    finalRot = Quaternion.LookRotation(hit.normal) * Quaternion.Euler(0, 0, currentRotationAngle);
                }
            }

            if (isValidSurfaceType)
            {
                currentPreview.transform.position = finalPos;
                currentPreview.transform.rotation = finalRot;
                CheckIfValid(finalPos, finalRot);
            }
            else
            {
                // If aiming at the wrong surface or an obstacle, I stick the preview raw and red
                currentPreview.transform.position = hit.point;

                if (currentItemData.placementType == PlacementType.Floor)
                    currentPreview.transform.rotation = Quaternion.Euler(0, currentRotationAngle, 0);
                else
                    currentPreview.transform.rotation = Quaternion.LookRotation(hit.normal);

                isValidPlacement = false;
                ChangePreviewColor(invalidMaterial);
            }
        }
        else
        {
            currentPreview.SetActive(false);
        }
    }

    private void CheckIfValid(Vector3 pos, Quaternion rot)
    {
        // FIX: I shrink the collision box by 5% so adjacent items don't falsely block each other
        Vector3 overlapExtents = currentItemData.extents * 0.95f;

        // I calculate the physical center of the mesh depending on its rotation
        Vector3 boxCenter = pos + (rot * currentItemData.centerOffset);

        bool hittingObstacles = Physics.CheckBox(boxCenter, overlapExtents, rot, obstacleLayer);

        isValidPlacement = !hittingObstacles;
        ChangePreviewColor(isValidPlacement ? validMaterial : invalidMaterial);
    }

    private void ChangePreviewColor(Material mat)
    {
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
        // I allow spinning the object (on the floor or spinning flat on the wall)
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentRotationAngle += 90f;
            if (currentRotationAngle >= 360f) currentRotationAngle = 0f;
        }
    }

    private void HandleClickToPlace()
    {
        if (Input.GetMouseButtonDown(0) && isValidPlacement && currentPreview.activeSelf)
        {
            GameObject placedObject = Instantiate(currentItemData.placementPrefab, currentPreview.transform.position, currentPreview.transform.rotation);

            int targetLayer = LayerMask.NameToLayer("NotPlaceableSurface");
            if (targetLayer != -1) SetLayerRecursively(placedObject, targetLayer);

            PlacedRewardBehavior behavior = placedObject.GetComponent<PlacedRewardBehavior>();
            if (behavior != null) behavior.rewardID = currentItemData.itemID;

            // I save it in the global persistent data
            GameManager.Instance.SavePlacedItem(currentItemData.itemID, placedObject.transform.position, placedObject.transform.rotation, Color.clear, -1f);

            Destroy(currentPreview);
            isPlacingMode = false;
        }
    }

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
                    GameManager.Instance.RemovePlacedItem(behavior.rewardID);
                    Destroy(behavior.gameObject);
                }
            }
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, newLayer);
    }
}