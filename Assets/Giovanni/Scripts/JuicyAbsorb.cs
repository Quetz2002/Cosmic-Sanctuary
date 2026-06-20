using UnityEngine;
using System;

namespace Giovanni.Gameplay
{
    public class JuicyAbsorb : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float initialSpeed = 3f;
        public float acceleration = 18f;
        public float rotationSpeed = 450f;
        public float completionDistance = 0.4f;

        [Header("Juice/Visual Easing")]
        [Tooltip("The height of the parabolic arc offset during flight.")]
        public float arcHeight = 1.2f;
        public AnimationCurve scaleCurve = AnimationCurve.Linear(0, 1, 1, 0);

        private Transform targetTransform;
        private Action onComplete;
        
        private float currentSpeed;
        private float progress = 0f;
        private Vector3 startPosition;
        private Vector3 currentLinearPosition;
        private Vector3 randomRotationAxis;
        private Vector3 originalScale;
        private bool isAbsorbing = false;

        private void Start()
        {
            originalScale = transform.localScale;
            // Generate a random axis to rotate around while flying for that chaotic cosmic look
            randomRotationAxis = UnityEngine.Random.onUnitSphere;
        }

        /// <summary>
        /// Starts the juicy absorption routine toward the target.
        /// </summary>
        public void Initiate(Transform target, Action callback)
        {
            targetTransform = target;
            onComplete = callback;
            startPosition = transform.position;
            currentLinearPosition = startPosition;
            currentSpeed = initialSpeed;
            progress = 0f;
            isAbsorbing = true;

            // Make sure the collider is off so it doesn't collide during flight
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }

        private void Update()
        {
            if (!isAbsorbing || targetTransform == null) return;

            // Update physical translation speed
            currentSpeed += acceleration * Time.deltaTime;

            // Calculate progress based on distance covered relative to start
            float totalDist = Vector3.Distance(startPosition, targetTransform.position);
            if (totalDist > 0.01f)
            {
                float step = currentSpeed * Time.deltaTime;
                currentLinearPosition = Vector3.MoveTowards(currentLinearPosition, targetTransform.position, step);
                
                // Track visual interpolation percentage (0 to 1)
                progress = Vector3.Distance(startPosition, currentLinearPosition) / totalDist;
                progress = Mathf.Clamp01(progress);

                // Add a parabolic height offset to simulate a dynamic suction arc
                float arcOffset = Mathf.Sin(progress * Mathf.PI) * arcHeight;
                transform.position = currentLinearPosition + (Vector3.up * arcOffset);
            }
            else
            {
                progress = 1f;
                transform.position = targetTransform.position;
            }

            // Spin the object around its random axis
            transform.Rotate(randomRotationAxis, rotationSpeed * Time.deltaTime, Space.World);

            // Scale down based on our animation curve
            transform.localScale = originalScale * scaleCurve.Evaluate(progress);

            // Check if arrived
            if (Vector3.Distance(transform.position, targetTransform.position) <= completionDistance || progress >= 0.99f)
            {
                isAbsorbing = false;
                onComplete?.Invoke();
            }
        }
    }
}
