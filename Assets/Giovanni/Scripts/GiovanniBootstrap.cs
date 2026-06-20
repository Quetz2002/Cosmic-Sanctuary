using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Giovanni.Gameplay
{
    public class GiovanniBootstrap : MonoBehaviour
    {
        [Header("Prefabs for Spawning (Optional)")]
        [Tooltip("If no Spawner exists in the scene, one will be created using these prefabs.")]
        public GameObject[] collectablePrefabs;
        
        [Tooltip("The BoxCollider zone boundary to assign to the Spawner if created.")]
        public BoxCollider customSpawnZone;

        [Header("UI Styling Settings")]
        public Font fontAsset; // Fallback font if TMPro default isn't loaded
        public Sprite crosshairSprite;

        private void Start()
        {
            SetupGameplayArchitecture();
        }

        [ContextMenu("Setup Gameplay Architecture")]
        public void SetupGameplayArchitecture()
        {
            Debug.Log("[GiovanniBootstrap] Starting scene setup...");

            // 1. Setup or Find Spawner
            ProceduralSpawner spawner = Object.FindFirstObjectByType<ProceduralSpawner>();
            if (spawner == null)
            {
                GameObject spawnerObj = new GameObject("Giovanni_ProceduralSpawner");
                spawner = spawnerObj.AddComponent<ProceduralSpawner>();
                spawner.prefabs = collectablePrefabs;
                
                if (customSpawnZone != null)
                {
                    spawner.spawnVolume = customSpawnZone;
                }
                else
                {
                    // Create a default spawn volume if none provided
                    BoxCollider box = spawnerObj.AddComponent<BoxCollider>();
                    box.size = new Vector3(40f, 2f, 40f);
                    box.isTrigger = true;
                    spawner.spawnVolume = box;
                }
                
                // Align defaults
                spawner.spawnCount = 10;
                spawner.groundLayer = LayerMask.GetMask("Default", "Placeable", "NotPlaceable");
                Debug.Log("[GiovanniBootstrap] Created new ProceduralSpawner GameObject.");
            }

            // 2. Setup or Find Collector on Player
            Collector collector = Object.FindFirstObjectByType<Collector>();
            if (collector == null)
            {
                // Try to find player by tag
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj == null)
                {
                    // Fallback to camera parent
                    Camera mainCam = Camera.main;
                    if (mainCam != null)
                    {
                        playerObj = mainCam.transform.parent != null ? mainCam.transform.parent.gameObject : mainCam.gameObject;
                    }
                }

                if (playerObj != null)
                {
                    collector = playerObj.AddComponent<Collector>();
                    Debug.Log($"[GiovanniBootstrap] Attached Collector script to: {playerObj.name}");
                }
                else
                {
                    Debug.LogWarning("[GiovanniBootstrap] Player GameObject not found! HUD will be created but not bound to a player yet.");
                }
            }

            // Get movement reference from Player for stamina mapping (null-safe)
            Movimiento movement = null;
            if (collector != null)
            {
                movement = collector.GetComponent<Movimiento>();
                if (movement == null)
                {
                    movement = collector.GetComponentInParent<Movimiento>();
                }
            }

            // 3. Create Canvas and HUD Components (Always create a dedicated Overlay canvas to prevent drawing in world-space or hidden canvases)
            GameObject hudCanvasObj = new GameObject("Giovanni_GameplayHUD");
            Canvas canvasComponent = hudCanvasObj.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasComponent.sortingOrder = 99; // Render on top of other menus
            
            CanvasScaler scaler = hudCanvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            hudCanvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("[GiovanniBootstrap] Created dedicated Screen-Space Canvas for GameplayHUD with 1920x1080 ScaleWithScreenSize setup.");

            // Add EventSystem if missing (needed for Canvas UI interaction)
            if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            // 4. Create UI Text & Slider Objects programmatically
            GameplayUI uiController = hudCanvasObj.GetComponent<GameplayUI>();
            if (uiController == null)
            {
                uiController = hudCanvasObj.AddComponent<GameplayUI>();
            }



            // Generate Panel container for Texts (placed top-left with 50px safe margins)
            GameObject panelObj = new GameObject("HUD_TextContainer", typeof(RectTransform), typeof(Image));
            panelObj.transform.SetParent(hudCanvasObj.transform, false);
            
            // Add a beautiful dark backing panel to enhance text readability on any planet terrain
            Image panelImage = panelObj.GetComponent<Image>();
            panelImage.color = new Color(0.02f, 0.02f, 0.02f, 0.65f); // Sleek dark glass effect
            
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0f, 1f); // Top Left
            panelRect.anchorMax = new Vector2(0f, 1f);
            panelRect.pivot = new Vector2(0f, 1f);
            panelRect.anchoredPosition = new Vector2(50f, -50f); // 50px margin
            panelRect.sizeDelta = new Vector2(320f, 130f); // Sleek sizing

            // Generate Text Elements inside the dark backing container with proper layout padding
            uiController.inventoryText = CreateTextElement(panelObj.transform, "InventoryText", "Cargando: 0/10", new Vector2(20f, -20f));
            uiController.scoreText = CreateTextElement(panelObj.transform, "ScoreText", "Entregados: 0", new Vector2(20f, -55f));
            uiController.remainingText = CreateTextElement(panelObj.transform, "RemainingText", "Restantes: 0", new Vector2(20f, -90f));

            // Generate Notification Text (centered at top-middle)
            GameObject notifyObj = new GameObject("HUD_NotificationText", typeof(RectTransform));
            notifyObj.transform.SetParent(hudCanvasObj.transform, false);
            RectTransform notifyRect = notifyObj.GetComponent<RectTransform>();
            notifyRect.anchorMin = new Vector2(0.5f, 0.8f); // Top center area (safely below top bezel)
            notifyRect.anchorMax = new Vector2(0.5f, 0.8f);
            notifyRect.pivot = new Vector2(0.5f, 0.5f);
            notifyRect.anchoredPosition = Vector2.zero;
            notifyRect.sizeDelta = new Vector2(600f, 60f);
            
            TMP_Text notifyTextComp = notifyObj.AddComponent<TextMeshProUGUI>();
            
            TMP_FontAsset defaultFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (defaultFont == null)
            {
                defaultFont = Resources.Load<TMP_FontAsset>("LiberationSans SDF");
            }
            if (defaultFont == null && TMP_Settings.defaultFontAsset != null)
            {
                defaultFont = TMP_Settings.defaultFontAsset;
            }

            if (defaultFont != null)
            {
                notifyTextComp.font = defaultFont;
            }

            notifyTextComp.alignment = TextAlignmentOptions.Center;
            notifyTextComp.fontSize = 22f;
            notifyTextComp.fontStyle = FontStyles.Bold;
            notifyTextComp.color = Color.yellow;
            uiController.notificationText = notifyTextComp;

            // Generate Stamina Bar slider (placed bottom-left with 50px safe margins)
            GameObject staminaBgObj = new GameObject("HUD_StaminaBar_BG", typeof(RectTransform), typeof(Image));
            staminaBgObj.transform.SetParent(hudCanvasObj.transform, false);
            RectTransform staminaBgRect = staminaBgObj.GetComponent<RectTransform>();
            staminaBgRect.anchorMin = new Vector2(0f, 0f); // Bottom Left
            staminaBgRect.anchorMax = new Vector2(0f, 0f);
            staminaBgRect.pivot = new Vector2(0f, 0f);
            staminaBgRect.anchoredPosition = new Vector2(50f, 50f); // 50px margin
            staminaBgRect.sizeDelta = new Vector2(250f, 18f); // Sleek size
            staminaBgObj.GetComponent<Image>().color = new Color(0.05f, 0.05f, 0.05f, 0.75f);

            GameObject staminaFillObj = new GameObject("HUD_StaminaBar_Fill", typeof(RectTransform), typeof(Image));
            staminaFillObj.transform.SetParent(staminaBgObj.transform, false);
            RectTransform staminaFillRect = staminaFillObj.GetComponent<RectTransform>();
            staminaFillRect.anchorMin = Vector2.zero;
            staminaFillRect.anchorMax = Vector2.one;
            staminaFillRect.sizeDelta = Vector2.zero;
            
            Image fillImage = staminaFillObj.GetComponent<Image>();
            fillImage.color = Color.cyan;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            uiController.staminaFillBar = fillImage;

            // 5. Generate Crosshair Image in center screen
            GameObject crosshairObj = new GameObject("HUD_Crosshair", typeof(RectTransform), typeof(Image));
            crosshairObj.transform.SetParent(hudCanvasObj.transform, false);
            RectTransform crosshairRect = crosshairObj.GetComponent<RectTransform>();
            crosshairRect.anchorMin = new Vector2(0.5f, 0.5f); // Perfect Center
            crosshairRect.anchorMax = new Vector2(0.5f, 0.5f);
            crosshairRect.pivot = new Vector2(0.5f, 0.5f);
            crosshairRect.anchoredPosition = Vector2.zero;
            crosshairRect.sizeDelta = new Vector2(20f, 20f);

            Image crossImage = crosshairObj.GetComponent<Image>();
            if (crosshairSprite != null)
            {
                crossImage.sprite = crosshairSprite;
            }
            else
            {
                // Simple default white block crosshair
                crossImage.color = Color.white;
            }

            // Attach CrosshairAnimator to Player or main camera (null-safe)
            if (collector != null)
            {
                GameObject animatorTarget = collector.gameObject;
                CrosshairAnimator crosshairAnimator = animatorTarget.GetComponent<CrosshairAnimator>();
                if (crosshairAnimator == null)
                {
                    crosshairAnimator = animatorTarget.AddComponent<CrosshairAnimator>();
                }
                crosshairAnimator.crosshairImage = crossImage;
            }

            // Initialize the UI controller programmatically to activate event subscriptions and sync texts
            uiController.Initialize(collector, spawner, movement);

            Debug.Log("[GiovanniBootstrap] HUD setup completed successfully! Architecture is wired and ready.");
        }

        private TMP_Text CreateTextElement(Transform parent, string name, string initialText, Vector2 pos)
        {
            GameObject textObj = new GameObject(name, typeof(RectTransform));
            textObj.transform.SetParent(parent, false);
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0f, 1f); // Top Left relative to parent
            textRect.anchorMax = new Vector2(0f, 1f);
            textRect.pivot = new Vector2(0f, 1f);
            textRect.anchoredPosition = pos;
            textRect.sizeDelta = new Vector2(300f, 30f);

            TMP_Text textComponent = textObj.AddComponent<TextMeshProUGUI>();
            
            // Explicitly load LiberationSans SDF with fallbacks to guarantee text renders
            TMP_FontAsset defaultFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (defaultFont == null)
            {
                defaultFont = Resources.Load<TMP_FontAsset>("LiberationSans SDF");
            }
            if (defaultFont == null && TMP_Settings.defaultFontAsset != null)
            {
                defaultFont = TMP_Settings.defaultFontAsset;
            }

            if (defaultFont != null)
            {
                textComponent.font = defaultFont;
            }

            textComponent.text = initialText;
            textComponent.fontSize = 18f;
            textComponent.color = Color.white;
            
            return textComponent;
        }
    }
}
