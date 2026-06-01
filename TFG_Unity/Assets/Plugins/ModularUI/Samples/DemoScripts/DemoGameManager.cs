using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

namespace ModularUIRuntime.Demo
{
    [ExecuteAlways]
    public partial class DemoGameManager : MonoBehaviour
    {
        [Header("UI")]
        public ModularHudController hudController;
        public ModularInventoryManager inventoryManager;
        public ModularPauseMenu pauseMenu;

        [Header("Player")]
        public Transform playerTransform;
        public float moveSpeed = 5f;
        public float lookSensitivity = 0.15f;

        [Header("Movement Settings")]
        public float jumpForce = 5f;
        public float gravity = -9.81f;
        private float verticalVelocity = 0f;
        private bool isGrounded = true;
        private float groundY = 1.5f;

        [Header("Inventory")]
        public Transform slotsContainer;
        public GameObject itemPrefab;

        [Header("Pickups")]
        public Transform pickupsParent;
        public float pickupRange = 3f;

        [Header("Objectives")]
        public List<string> objectives = new List<string>();

        [Header("Input")]
        public InputActionAsset inputActions;

        float currentHealth = 1f;
        float currentMana = 1f;
        int currentObjective = 0;
        int collected = 0;
        bool inventoryOpen = false;
        bool gamePaused = false;
        float yaw, pitch;
        DemoPickup nearestPickup;
        DemoPickup lastDetectedPickup;
        Canvas inventoryCanvas;
        MobileTouchInput mobileInput;
        InputAction moveAction, lookAction, interactAction, cancelAction;

        void Start()
        {
            if (!Application.isPlaying) return;

#if UNITY_EDITOR
            if (inputActions == null)
            {
                inputActions = UnityEditor.AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Plugins/ModularUI/Samples/InputSystem_Actions.inputactions");
                if (inputActions == null)
                {
                    inputActions = UnityEditor.AssetDatabase.LoadAssetAtPath<InputActionAsset>("Packages/com.pau.modularui/Samples/InputSystem_Actions.inputactions");
                }
            }
#endif

            if (playerTransform != null)
            {
                groundY = playerTransform.position.y;
            }

            if (hudController == null)
            {
                hudController = FindAnyObjectByType<ModularHudController>(FindObjectsInactive.Include);
            }
            if (inventoryManager == null)
            {
                inventoryManager = FindAnyObjectByType<ModularInventoryManager>(FindObjectsInactive.Include);
            }
            if (pauseMenu == null)
            {
                pauseMenu = FindAnyObjectByType<ModularPauseMenu>(FindObjectsInactive.Include);
            }

            if (hudController != null)
            {
                hudController.gameObject.SetActive(true);
                Transform parent = hudController.transform.parent;
                while (parent != null)
                {
                    parent.gameObject.SetActive(true);
                    parent = parent.parent;
                }
            }

            if (inputActions != null)
            {
                var playerMap = inputActions.FindActionMap("Player");
                if (playerMap != null)
                {
                    moveAction = playerMap.FindAction("Move");
                    lookAction = playerMap.FindAction("Look");
                    interactAction = playerMap.FindAction("Interact");
                    playerMap.Enable();
                }
                var uiMap = inputActions.FindActionMap("UI");
                if (uiMap != null)
                {
                    cancelAction = uiMap.FindAction("Cancel");
                    uiMap.Enable();
                }
            }

            mobileInput = FindFirstObjectByType<MobileTouchInput>();

            bool isMobile = GetPlatform() == UIConfiguration.TargetPlatform.MobilePortrait ||
                            GetPlatform() == UIConfiguration.TargetPlatform.MobileLandscape;

            if (isMobile)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                SetupMobileControls();
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            hudController?.SetHealth(1f);
            hudController?.SetSecondaryResource(1f);

            objectives = new List<string>
            {
                "Explore the triggers",
                "Collect all items"
            };

            SetObjective(objectives[0]);

            if (inventoryManager != null)
            {
                inventoryManager.gameObject.SetActive(true);
                Transform parent = inventoryManager.transform.parent;
                while (parent != null)
                {
                    parent.gameObject.SetActive(true);
                    parent = parent.parent;
                }
                inventoryManager.gameObject.SetActive(false);
                inventoryOpen = false;
            }

            if (pauseMenu != null)
            {
                pauseMenu.gameObject.SetActive(true);
                Transform parent = pauseMenu.transform.parent;
                while (parent != null)
                {
                    parent.gameObject.SetActive(true);
                    parent = parent.parent;
                }
                pauseMenu.Resume();
            }

            SetupPlayerPhysics();
            SetupPlayerCamera();
            SetupMapColorChanger();
            SetupSimplifiedElements();
            FixRenderPipelineShaders();

            hudController?.ShowSimplePopUp("Use WASD/Joystick to move, E to collect, I for inventory", 5f);
        }

        void Update()
        {
            if (!Application.isPlaying) return;

            if (inventoryManager != null)
            {
                if (inventoryManager.gameObject.activeSelf != inventoryOpen)
                {
                    inventoryOpen = inventoryManager.gameObject.activeSelf;
                    if (GetPlatform() == UIConfiguration.TargetPlatform.Desktop)
                    {
                        Cursor.lockState = inventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
                        Cursor.visible = inventoryOpen;
                    }
                }
            }

            if (pauseMenu != null)
            {
                bool pausePanelVisible = pauseMenu.IsPanelActive && pauseMenu.gameObject.activeInHierarchy;
                if (gamePaused && (!pausePanelVisible || Time.timeScale == 1f))
                {
                    gamePaused = false;
                    Time.timeScale = 1f;
                    if (GetPlatform() == UIConfiguration.TargetPlatform.Desktop)
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                }
                else if (!gamePaused && pausePanelVisible)
                {
                    gamePaused = true;
                    Time.timeScale = 0f;
                    if (GetPlatform() == UIConfiguration.TargetPlatform.Desktop)
                    {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }
                }
            }

            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && !gamePaused && !inventoryOpen)
            {
                Jump();
            }

            HandleMovement();
            HandlePickupDetection();

            if (interactAction != null && interactAction.WasPressedThisFrame() && nearestPickup != null)
            {
                CollectItem(nearestPickup);
            }

            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (inventoryOpen)
                {
                    ToggleInventory();
                }
                else
                {
                    TogglePauseMenu();
                }
            }

            if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame && !gamePaused)
            {
                ToggleInventory();
            }

            if (IsMobilePlatform())
            {
                HandleMobileLook();
            }

            if (!gamePaused && !inventoryOpen)
            {
                if (GetPlatform() == UIConfiguration.TargetPlatform.Desktop && Cursor.lockState != CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }

        public void ToggleInventory()
        {
            inventoryOpen = !inventoryOpen;
            if (inventoryManager != null)
            {
                inventoryManager.gameObject.SetActive(inventoryOpen);
            }

            if (GetPlatform() == UIConfiguration.TargetPlatform.Desktop)
            {
                Cursor.lockState = inventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = inventoryOpen;
            }
        }

        public void TogglePauseMenu()
        {
            if (pauseMenu == null)
            {
                return;
            }

            if (!pauseMenu.gameObject.activeInHierarchy)
            {
                pauseMenu.gameObject.SetActive(true);
                Transform parent = pauseMenu.transform.parent;
                while (parent != null)
                {
                    parent.gameObject.SetActive(true);
                    parent = parent.parent;
                }
            }

            pauseMenu.TogglePause();
            gamePaused = pauseMenu.IsPanelActive;

            if (GetPlatform() == UIConfiguration.TargetPlatform.Desktop)
            {
                Cursor.lockState = gamePaused ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = gamePaused;
            }
        }

        void CollectItem(DemoPickup pickup)
        {
            if (pickup == null || pickup.itemData == null)
            {
                return;
            }
            if (!AddItemToInventory(pickup.itemData, pickup.quantity))
            {
                hudController?.ShowSimplePopUp("Inventory full!", 2f);
                return;
            }

            pickup.Collect();
            collected++;
            hudController?.ShowSimplePopUp("Collected: " + pickup.itemData.itemName, 2f);
            ApplyEffect(pickup);
            CheckObjectives();
        }

        bool AddItemToInventory(ItemData data, int qty)
        {
            if (slotsContainer == null || itemPrefab == null)
            {
                return false;
            }

            if (data.isStackable)
            {
                foreach (var item in slotsContainer.GetComponentsInChildren<ModularInventoryItem>())
                {
                    if (item.data == data)
                    {
                        item.currentQuantity += qty;
                        item.RefreshVisual();
                        return true;
                    }
                }
            }

            foreach (Transform slot in slotsContainer)
            {
                if (slot.GetComponent<ModularInventorySlot>() == null || slot.childCount > 0)
                {
                    continue;
                }
                var obj = Instantiate(itemPrefab, slot);
                var item = obj.GetComponent<ModularInventoryItem>();
                if (item != null)
                {
                    item.data = data;
                    item.currentQuantity = qty;
                    item.RefreshVisual();
                }
                var r = obj.GetComponent<RectTransform>();
                if (r != null)
                {
                    r.anchorMin = r.anchorMax = r.pivot = new Vector2(0.5f, 0.5f);
                    r.anchoredPosition = Vector2.zero;
                }
                return true;
            }
            return false;
        }

        void ApplyEffect(DemoPickup pickup)
        {
            switch (pickup.pickupEffect)
            {
                case DemoPickup.PickupEffect.HealPlayer:
                    Heal(pickup.effectValue);
                    break;
                case DemoPickup.PickupEffect.RestoreMana:
                    RestoreMana(pickup.effectValue);
                    break;
                case DemoPickup.PickupEffect.DamagePlayer:
                    ApplyDamage(pickup.effectValue);
                    break;
            }
        }

        public void OnItemUsedFromInventory(ItemData item)
        {
            if (item == null)
            {
                return;
            }
            hudController?.ShowSimplePopUp("Used: " + item.itemName, 2f);
            if (item.type == ItemType.Consumable)
            {
                Heal(0.25f);
                RestoreMana(0.15f);
            }
        }

        public void OnItemDroppedFromInventory(ItemData item)
        {
            if (item != null)
            {
                hudController?.ShowSimplePopUp("Dropped: " + item.itemName, 2f);
            }
        }

        void SetObjective(string text)
        {
            hudController?.SetObjective(text);
        }

        void CheckObjectives()
        {
            if (currentObjective >= objectives.Count)
            {
                return;
            }
            currentObjective++;
            if (currentObjective < objectives.Count)
            {
                SetObjective(objectives[currentObjective]);
            }
            else
            {
                SetObjective("All objectives complete!");
                hudController?.ShowImportantPopUp("Demo Complete!", 5f);
            }
        }

        UIConfiguration.TargetPlatform GetPlatform()
        {
            if (ModularThemeManager.Instance != null && ModularThemeManager.Instance.Config != null)
            {
                return ModularThemeManager.Instance.Config.selectedPlatform;
            }
            return UIConfiguration.TargetPlatform.Desktop;
        }

        bool IsMobilePlatform()
        {
            var p = GetPlatform();
            return p == UIConfiguration.TargetPlatform.MobilePortrait || p == UIConfiguration.TargetPlatform.MobileLandscape;
        }
    }
}
