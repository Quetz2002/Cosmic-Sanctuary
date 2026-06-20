using UnityEngine;
using System;

namespace Giovanni.Gameplay
{
    public class Collector : MonoBehaviour
    {
        [Header("Tuning Settings")]
        public float maxGrabDistance = 4f;
        public int inventoryCapacity = 10;
        public Camera playerCamera;

        [Header("State Values (Read-Only in Inspector)")]
        [SerializeField] private int currentHolding = 0;
        [SerializeField] private int totalScore = 0;
        [SerializeField] private bool inDeliveryZone = false;

        // Reactive Events for Decoupled architecture
        public event Action<int, int> OnInventoryChanged; // (current, maxCapacity)
        public event Action<int> OnScoreChanged;           // (totalScore)
        public event Action<string> OnNotificationTriggered; // Inform UI/Player of events

        private Controles controls;

        public int CurrentHolding => currentHolding;
        public int TotalScore => totalScore;
        public int InventoryCapacity => inventoryCapacity;

        private void Awake()
        {
            controls = new Controles();
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }
        }

        private void OnEnable()
        {
            if (controls != null)
            {
                controls.Jugador.Enable();
                controls.Jugador.Recolectar.performed += Recolectar_performed;
                controls.Jugador.Depositar.performed += Depositar_performed;
            }
        }

        private void OnDisable()
        {
            if (controls != null)
            {
                controls.Jugador.Recolectar.performed -= Recolectar_performed;
                controls.Jugador.Depositar.performed -= Depositar_performed;
                controls.Jugador.Disable();
            }
        }

        private void Recolectar_performed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            TryCollect();
        }

        private void Depositar_performed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            TryDeposit();
        }

        private void Start()
        {
            // Trigger initial events to align UI on startup
            OnInventoryChanged?.Invoke(currentHolding, inventoryCapacity);
            OnScoreChanged?.Invoke(totalScore);
        }

        private void TryCollect()
        {
            if (currentHolding >= inventoryCapacity)
            {
                OnNotificationTriggered?.Invoke("Inventory FULL! Deposit items at the Drop Zone.");
                return;
            }

            if (playerCamera == null) return;

            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxGrabDistance))
            {
                CollectableItem collectable = hit.collider.GetComponent<CollectableItem>();
                if (collectable != null)
                {
                    // Start suction animation if not already being collected
                    if (collectable.InitiateCollection())
                    {
                        JuicyAbsorb absorber = collectable.GetComponent<JuicyAbsorb>();
                        if (absorber != null)
                        {
                            // Fly towards player camera
                            absorber.Initiate(playerCamera.transform, () => FinalizeItemCollection(collectable));
                        }
                        else
                        {
                            // Immediate fallback if no visual script exists
                            FinalizeItemCollection(collectable);
                        }
                    }
                }
            }
        }

        private void FinalizeItemCollection(CollectableItem collectable)
        {
            if (collectable == null) return;

            // Trigger visual collection burst and get value
            int value = collectable.FinalizeCollection();
            currentHolding += value;
            currentHolding = Mathf.Clamp(currentHolding, 0, inventoryCapacity);

            // Find Spawner and report collection
            ProceduralSpawner spawner = UnityEngine.Object.FindFirstObjectByType<ProceduralSpawner>();
            if (spawner != null)
            {
                spawner.UnregisterItem(collectable.gameObject);
            }

            // Notify listeners of change
            OnInventoryChanged?.Invoke(currentHolding, inventoryCapacity);
            OnNotificationTriggered?.Invoke($"+{value} Collected");

            // Deactivate and handle the GameObject
            collectable.gameObject.SetActive(false);
        }

        private void TryDeposit()
        {
            if (!inDeliveryZone)
            {
                OnNotificationTriggered?.Invoke("Must be in the Drop Zone to deliver items!");
                return;
            }

            if (currentHolding <= 0)
            {
                OnNotificationTriggered?.Invoke("No items to deposit!");
                return;
            }

            // Move holding items to global score
            int depositedCount = currentHolding;
            totalScore += depositedCount;
            currentHolding = 0;

            // Sync with global GameManager cosmic materials currency
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddMaterial(depositedCount);
            }

            // Notify UI & Systems
            OnInventoryChanged?.Invoke(currentHolding, inventoryCapacity);
            OnScoreChanged?.Invoke(totalScore);
            OnNotificationTriggered?.Invoke($"Deposited {depositedCount} items! Total score: {totalScore}");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ZonaEntrega"))
            {
                inDeliveryZone = true;
                OnNotificationTriggered?.Invoke("Entered Drop Zone. Press Depositar key to secure cargo!");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("ZonaEntrega"))
            {
                inDeliveryZone = false;
                OnNotificationTriggered?.Invoke("Left Drop Zone.");
            }
        }
    }
}
