using UnityEngine;
using UnityEngine.UI;

namespace Giovanni.Gameplay
{
    public class CrosshairAnimator : MonoBehaviour
    {
        [Header("Target UI")]
        public Image crosshairImage;

        [Header("Tuning Settings")]
        public float maxDetectionDistance = 4f;
        public float animationSpeed = 12f;

        [Header("Visual States")]
        public Color normalColor = Color.white;
        public Color highlightedColor = Color.green;
        public Vector3 normalScale = Vector3.one;
        public Vector3 highlightedScale = new Vector3(1.25f, 1.25f, 1.25f);

        private Camera playerCamera;
        private Vector3 targetScale;
        private Color targetColor;

        private void Start()
        {
            playerCamera = Camera.main;

            if (crosshairImage == null)
            {
                crosshairImage = GetComponent<Image>();
            }

            if (crosshairImage != null)
            {
                crosshairImage.color = normalColor;
                crosshairImage.transform.localScale = normalScale;
            }

            targetScale = normalScale;
            targetColor = normalColor;
        }

        private void Update()
        {
            if (playerCamera == null) return;

            // Perform raycast check from camera forward direction
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;
            bool isAimingAtCollectable = false;

            if (Physics.Raycast(ray, out hit, maxDetectionDistance))
            {
                if (hit.collider.GetComponent<CollectableItem>() != null)
                {
                    isAimingAtCollectable = true;
                }
            }

            // Set targets based on state
            targetScale = isAimingAtCollectable ? highlightedScale : normalScale;
            targetColor = isAimingAtCollectable ? highlightedColor : normalColor;

            // Interpolate values smoothly using Lerp for juicy feel
            if (crosshairImage != null)
            {
                crosshairImage.transform.localScale = Vector3.Lerp(
                    crosshairImage.transform.localScale, 
                    targetScale, 
                    Time.deltaTime * animationSpeed
                );

                crosshairImage.color = Color.Lerp(
                    crosshairImage.color, 
                    targetColor, 
                    Time.deltaTime * animationSpeed
                );
            }
        }
    }
}
