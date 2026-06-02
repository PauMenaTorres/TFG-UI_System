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

        void Start()
        {
            if (!Application.isPlaying) return;

            if (playerTransform == null)
            {
                GameObject playerGo = GameObject.Find("Player");
                if (playerGo != null)
                {
                    playerTransform = playerGo.transform;
                }
            }

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
                try
                {
                    hudController.gameObject.SetActive(true);
                    Transform parent = hudController.transform.parent;
                    while (parent != null)
                    {
                        parent.gameObject.SetActive(true);
                        parent = parent.parent;
                    }
                }
                catch (System.Exception)
                {
                    hudController = null;
                }
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

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
                try
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
                catch (System.Exception)
                {
                    inventoryManager = null;
                }
            }

            if (pauseMenu != null)
            {
                try
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
                catch (System.Exception)
                {
                    pauseMenu = null;
                }
            }

            if (pickupsParent == null)
            {
                GameObject pickupsGo = GameObject.Find("Pickups");
                if (pickupsGo != null)
                {
                    pickupsParent = pickupsGo.transform;
                }
                else
                {
                    pickupsGo = new GameObject("Pickups");
                    pickupsParent = pickupsGo.transform;
                }
            }

            if (slotsContainer == null && inventoryManager != null)
            {
                var slot = inventoryManager.GetComponentInChildren<ModularInventorySlot>(true);
                if (slot != null)
                {
                    slotsContainer = slot.transform.parent;
                }
            }

            if (itemPrefab == null)
            {
                itemPrefab = Resources.Load<GameObject>("InventoryItem");
#if UNITY_EDITOR
                if (itemPrefab == null)
                {
                    string[] guids = UnityEditor.AssetDatabase.FindAssets("InventoryItem t:Prefab");
                    if (guids != null && guids.Length > 0)
                    {
                        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                        itemPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    }
                }
#endif
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
            if (playerTransform == null) return;

            var kb = Keyboard.current;
            var mouse = Mouse.current;
            if (kb == null) return;

            // Movement (WASD)
            if (!gamePaused && !inventoryOpen)
            {
                float mx = 0, my = 0;
                if (kb.wKey.isPressed) my += 1f;
                if (kb.sKey.isPressed) my -= 1f;
                if (kb.aKey.isPressed) mx -= 1f;
                if (kb.dKey.isPressed) mx += 1f;

                // Look (Mouse)
                Vector2 look = mouse != null ? mouse.delta.ReadValue() : Vector2.zero;
                yaw += look.x * lookSensitivity;
                pitch -= look.y * lookSensitivity;
                pitch = Mathf.Clamp(pitch, -90f, 90f);
                playerTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);

                // Apply movement
                Vector3 fwd = playerTransform.forward; fwd.y = 0; fwd.Normalize();
                Vector3 right = playerTransform.right; right.y = 0; right.Normalize();

                // Gravity / Jump
                if (playerTransform.position.y > groundY + 0.05f)
                {
                    verticalVelocity += gravity * Time.deltaTime;
                    isGrounded = false;
                }
                else
                {
                    if (playerTransform.position.y < groundY)
                    {
                        Vector3 p = playerTransform.position; p.y = groundY; playerTransform.position = p;
                    }
                    verticalVelocity = 0f;
                    isGrounded = true;
                }

                if (kb.spaceKey.wasPressedThisFrame && isGrounded) { verticalVelocity = jumpForce; isGrounded = false; }

                Vector3 delta = (fwd * my + right * mx) * moveSpeed * Time.deltaTime;
                delta.y = verticalVelocity * Time.deltaTime;
                playerTransform.position += delta;

                // Lock cursor while playing
                if (Cursor.lockState != CursorLockMode.Locked) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
            }

            // Pickup detection
            HandlePickupDetection();
            if (kb.eKey.wasPressedThisFrame && nearestPickup != null) CollectItem(nearestPickup);

            // Toggle Inventory (I)
            if (kb.iKey.wasPressedThisFrame && !gamePaused) ToggleInventory();

            // Toggle Pause / Close Inventory (ESC)
            if (kb.escapeKey.wasPressedThisFrame)
            {
                if (inventoryOpen) ToggleInventory();
                else TogglePauseMenu();
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
