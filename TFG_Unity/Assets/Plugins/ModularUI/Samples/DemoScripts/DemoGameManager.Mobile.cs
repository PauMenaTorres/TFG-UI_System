using UnityEngine;
using System.Collections;

namespace ModularUIRuntime.Demo
{
    public partial class DemoGameManager
    {
        void SetupMobileControls()
        {
            MobileControlRequirement req = FindFirstObjectByType<MobileControlRequirement>();
            if (req == null)
            {
                GameObject reqGo = new GameObject("MobileControlRequirement");
                req = reqGo.AddComponent<MobileControlRequirement>();
                req.mode = UIConfiguration.MobileControlMode.StandardGameplay;
            }

            StartCoroutine(SpawnMobileControlsDelayed());
        }

        IEnumerator SpawnMobileControlsDelayed()
        {
            yield return null;

            mobileInput = FindFirstObjectByType<MobileTouchInput>();

            if (mobileInput == null)
            {
                UIConfiguration config = null;
                if (ModularThemeManager.HasInstance && ModularThemeManager.Instance?.Config != null)
                {
                    config = ModularThemeManager.Instance.Config;
                }
                else
                {
                    ModularCanvasInitializer init = FindFirstObjectByType<ModularCanvasInitializer>();
                    if (init != null)
                    {
                        config = init.config;
                    }
                }

                if (config != null && config.mobileControlsPrefab != null)
                {
                    Canvas targetCanvas = FindFirstObjectByType<Canvas>();
                    if (targetCanvas != null)
                    {
                        GameObject controls = Instantiate(config.mobileControlsPrefab, targetCanvas.transform);
                        controls.name = config.mobileControlsPrefab.name;
                        mobileInput = controls.GetComponent<MobileTouchInput>();
                        if (mobileInput == null)
                        {
                            mobileInput = controls.GetComponentInChildren<MobileTouchInput>();
                        }
                    }
                }
            }

            if (mobileInput != null)
            {
                mobileInput.EnableInput();
                mobileInput.OnJump -= HandleMobileInteract;
                mobileInput.OnJump += HandleMobileInteract;
                mobileInput.OnJump -= Jump;
                mobileInput.OnJump += Jump;
                mobileInput.OnCancelPressed -= HandleMobileCancel;
                mobileInput.OnCancelPressed += HandleMobileCancel;
                mobileInput.OnMenuTogglePressed -= HandleMobileMenu;
                mobileInput.OnMenuTogglePressed += HandleMobileMenu;
            }
        }

        void HandleMobileInteract()
        {
            if (nearestPickup != null)
            {
                CollectItem(nearestPickup);
            }
        }

        void HandleMobileCancel()
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

        void HandleMobileMenu()
        {
            if (!gamePaused)
            {
                ToggleInventory();
            }
        }

        int lastTouchId = -1;
        Vector2 lastTouchPos;

        void HandleMobileLook()
        {
            if (playerTransform == null || inventoryOpen || gamePaused)
            {
                return;
            }

            if (UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.enabled == false)
            {
                UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
            }

            var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
            for (int i = 0; i < touches.Count; i++)
            {
                var t = touches[i];
                if (t.startScreenPosition.x < Screen.width * 0.5f)
                {
                    continue;
                }

                if (t.phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    lastTouchId = t.touchId;
                    lastTouchPos = t.screenPosition;
                }
                else if (t.touchId == lastTouchId && (t.phase == UnityEngine.InputSystem.TouchPhase.Moved || t.phase == UnityEngine.InputSystem.TouchPhase.Stationary))
                {
                    Vector2 delta = t.screenPosition - lastTouchPos;
                    lastTouchPos = t.screenPosition;
                    yaw += delta.x * lookSensitivity * 0.5f;
                    pitch -= delta.y * lookSensitivity * 0.5f;
                    pitch = Mathf.Clamp(pitch, -90f, 90f);
                    playerTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);
                }
                else if (t.touchId == lastTouchId && (t.phase == UnityEngine.InputSystem.TouchPhase.Ended || t.phase == UnityEngine.InputSystem.TouchPhase.Canceled))
                {
                    lastTouchId = -1;
                }
            }
        }

        void SetupMobileControlsInEditor()
        {
            bool isMobile = IsMobilePlatform();
            
            MobileTouchInput existing = FindFirstObjectByType<MobileTouchInput>(FindObjectsInactive.Include);
            
            if (isMobile)
            {
                if (existing == null)
                {
                    UIConfiguration config = null;
                    if (ModularThemeManager.HasInstance && ModularThemeManager.Instance?.Config != null)
                    {
                        config = ModularThemeManager.Instance.Config;
                    }
                    else
                    {
                        ModularCanvasInitializer init = FindFirstObjectByType<ModularCanvasInitializer>(FindObjectsInactive.Include);
                        if (init != null)
                        {
                            config = init.config;
                        }
                    }

                    if (config != null && config.mobileControlsPrefab != null)
                    {
                        Canvas targetCanvas = FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
                        if (targetCanvas != null)
                        {
                            GameObject controls = null;
#if UNITY_EDITOR
                            controls = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(config.mobileControlsPrefab, targetCanvas.transform);
#else
                            controls = Instantiate(config.mobileControlsPrefab, targetCanvas.transform);
#endif
                            if (controls != null)
                            {
                                controls.name = config.mobileControlsPrefab.name;
                                controls.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                                controls.transform.localScale = Vector3.one;
                                existing = controls.GetComponent<MobileTouchInput>();
                                if (existing == null)
                                {
                                    existing = controls.GetComponentInChildren<MobileTouchInput>();
                                }
#if UNITY_EDITOR
                                UnityEditor.Undo.RegisterCreatedObjectUndo(controls, "Create Mobile Controls");
                                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#endif
                            }
                        }
                    }
                }
                
                if (existing != null && !existing.gameObject.activeSelf)
                {
                    existing.gameObject.SetActive(true);
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(existing.gameObject);
#endif
                }
            }
            else
            {
                if (existing != null)
                {
                    Transform rootToDestroy = existing.transform;
                    while (rootToDestroy.parent != null && rootToDestroy.parent.GetComponent<Canvas>() == null)
                    {
                        rootToDestroy = rootToDestroy.parent;
                    }

#if UNITY_EDITOR
                    UnityEditor.Undo.DestroyObjectImmediate(rootToDestroy.gameObject);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#else
                    DestroyImmediate(rootToDestroy.gameObject);
#endif
                }

                Canvas targetCanvas = FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
                if (targetCanvas != null)
                {
                    Transform mobileControlsTrans = targetCanvas.transform.Find("MobileControls");
                    if (mobileControlsTrans != null)
                    {
#if UNITY_EDITOR
                        UnityEditor.Undo.DestroyObjectImmediate(mobileControlsTrans.gameObject);
                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#else
                        DestroyImmediate(mobileControlsTrans.gameObject);
#endif
                    }
                }
            }
        }
    }
}
