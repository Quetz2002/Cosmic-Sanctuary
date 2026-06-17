using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Giovanni.Gameplay
{
    public class GameplayUI : MonoBehaviour
    {
        [Header("References")]
        public Collector collector;
        public ProceduralSpawner spawner;
        public Movimiento movementController; // Legacy or new movement controller

        [Header("UI Outlets")]
        public Image staminaFillBar;
        public TMP_Text inventoryText;
        public TMP_Text scoreText;
        public TMP_Text remainingText;

        [Header("Notifications")]
        public TMP_Text notificationText;
        public float notificationDuration = 3f;

        private Coroutine notificationFadeRoutine;

        private void OnEnable()
        {
            if (collector != null)
            {
                collector.OnInventoryChanged += UpdateInventoryText;
                collector.OnScoreChanged += UpdateScoreText;
                collector.OnNotificationTriggered += ShowNotification;
            }

            if (spawner != null)
            {
                spawner.OnRemainingCountChanged += UpdateRemainingText;
                spawner.OnAllCollected += HandleAllCollected;
            }
        }

        private void OnDisable()
        {
            if (collector != null)
            {
                collector.OnInventoryChanged -= UpdateInventoryText;
                collector.OnScoreChanged -= UpdateScoreText;
                collector.OnNotificationTriggered -= ShowNotification;
            }

            if (spawner != null)
            {
                spawner.OnRemainingCountChanged -= UpdateRemainingText;
                spawner.OnAllCollected -= HandleAllCollected;
            }
        }

        public void Initialize(Collector newCollector, ProceduralSpawner newSpawner, Movimiento newMovement)
        {
            OnDisable(); // Unsubscribe from existing events if any

            collector = newCollector;
            spawner = newSpawner;
            movementController = newMovement;

            OnEnable(); // Subscribe to new events
            
            // Sync initial values immediately
            if (collector != null)
            {
                UpdateInventoryText(collector.CurrentHolding, collector.InventoryCapacity);
                UpdateScoreText(collector.TotalScore);
            }

            if (spawner != null)
            {
                UpdateRemainingText(spawner.GetRemainingCount());
            }
        }

        private void Start()
        {
            // Initial sync in case objects started with values
            if (collector != null)
            {
                UpdateInventoryText(collector.CurrentHolding, collector.InventoryCapacity);
                UpdateScoreText(collector.TotalScore);
            }

            if (spawner != null)
            {
                UpdateRemainingText(spawner.GetRemainingCount());
            }

            if (notificationText != null)
            {
                notificationText.text = "";
            }
        }

        private void Update()
        {
            // Poll stamina only since it changes continuously during sprint
            if (staminaFillBar != null && movementController != null)
            {
                staminaFillBar.fillAmount = movementController.ObtenerEstaminaNormalizada();
            }
        }

        private void UpdateInventoryText(int current, int maxCapacity)
        {
            if (inventoryText != null)
            {
                inventoryText.text = $"Cargando: {current}/{maxCapacity}";
            }
        }

        private void UpdateScoreText(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Entregados: {score}";
            }
        }

        private void UpdateRemainingText(int remaining)
        {
            if (remainingText != null)
            {
                remainingText.text = $"Restantes: {remaining}";
            }
        }

        private void HandleAllCollected()
        {
            ShowNotification("EXCELENTE! Has recolectado todos los minerales de la zona.");
        }

        public void ShowNotification(string message)
        {
            if (notificationText == null) return;

            if (notificationFadeRoutine != null)
            {
                StopCoroutine(notificationFadeRoutine);
            }

            notificationFadeRoutine = StartCoroutine(AnimateNotification(message));
        }

        private IEnumerator AnimateNotification(string message)
        {
            notificationText.text = message;
            notificationText.color = new Color(notificationText.color.r, notificationText.color.g, notificationText.color.b, 1f);

            yield return new WaitForSeconds(notificationDuration - 0.5f);

            // Smooth fade out
            float elapsed = 0f;
            float fadeTime = 0.5f;
            Color originalColor = notificationText.color;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
                notificationText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }

            notificationText.text = "";
        }
    }
}
