using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ModularUIRuntime
{
    public abstract class BasePlatformUIAdapter : IPlatformUIAdapter
    {
        public abstract void SetupCanvas(Canvas targetCanvas);

        protected bool IsPrefabStageOrAsset(GameObject go)
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorUtility.IsPersistent(go))
            {
                return true;
            }
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(go) != null)
            {
                return true;
            }
#endif
            return false;
        }

        protected bool IsVRCamera(Camera cam)
        {
            if (cam == null) return false;
            string rootName = cam.transform.root.name.ToLower();
            if (rootName.Contains("ovrcamerarig") || rootName.Contains("vrrig") || rootName.Contains("xr-rig") || rootName.Contains("xrrig"))
            {
                return true;
            }

            Component[] allComponents = cam.transform.root.GetComponentsInChildren<Component>(true);
            foreach (var comp in allComponents)
            {
                if (comp == null) continue;
                string typeName = comp.GetType().FullName;
                if (string.IsNullOrEmpty(typeName)) continue;

                if (typeName.Contains("OVR") || typeName.Contains("Oculus") || typeName.Contains("Locomotor") || typeName.Contains("XRRig") || typeName.Contains("XROrigin"))
                {
                    return true;
                }
            }

            return false;
        }

        protected void CleanupVREnvironment(Canvas targetCanvas)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && IsPrefabStageOrAsset(targetCanvas.gameObject))
            {
                return;
            }
#endif

            Component ovrRaycaster = targetCanvas.GetComponent("OVRRaycaster");
            if (ovrRaycaster != null)
            {
                if (Application.isPlaying) Object.Destroy(ovrRaycaster);
                else Object.DestroyImmediate(ovrRaycaster, true);
            }

            Component ovrOverlay = targetCanvas.GetComponent("OVROverlayCanvas");
            if (ovrOverlay == null) ovrOverlay = targetCanvas.GetComponent("Oculus.Interaction.OVROverlayCanvas");
            if (ovrOverlay != null)
            {
                if (Application.isPlaying) Object.Destroy(ovrOverlay);
                else Object.DestroyImmediate(ovrOverlay, true);
            }

            var allGOs = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var go in allGOs)
            {
                if (go != null && (go.name == "ISDK_RayCanvasInteraction" || go.name == "ISDK_PokeCanvasInteraction"))
                {
                    if (go.transform.parent == targetCanvas.transform || go.transform.parent == null)
                    {
                        if (Application.isPlaying) Object.Destroy(go);
                        else Object.DestroyImmediate(go, true);
                    }
                }
            }

            if (targetCanvas.worldCamera != null && IsVRCamera(targetCanvas.worldCamera))
            {
                targetCanvas.worldCamera = null;
            }
            
            Camera[] allCameras = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (Camera cam in allCameras)
            {
                if (IsVRCamera(cam))
                {
                    if (Application.isPlaying) Object.Destroy(cam.transform.root.gameObject);
                    else Object.DestroyImmediate(cam.transform.root.gameObject, true);
                }
            }

            Camera mainCam = Object.FindFirstObjectByType<Camera>();
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
                camObj.transform.position = new Vector3(0, 1, -10);

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.Undo.RegisterCreatedObjectUndo(camObj, "Create Main Camera");
                }
#endif
            }

            EnsureStandardEventSystem(targetCanvas.gameObject);
        }

        protected void EnsureStandardEventSystem(GameObject canvasGo)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && IsPrefabStageOrAsset(canvasGo))
            {
                return;
            }
#endif

            EventSystem currentES = Object.FindFirstObjectByType<EventSystem>();
            
            if (currentES != null)
            {
                System.Type standardInputType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
                bool hasStandardInput = false;
                if (standardInputType != null && currentES.GetComponent(standardInputType) != null)
                {
                    hasStandardInput = true;
                }
                else if (currentES.GetComponent<StandaloneInputModule>() != null)
                {
                    hasStandardInput = true;
                }

                if (!hasStandardInput)
                {
                    AddStandardInputModule(currentES.gameObject);
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        UnityEditor.EditorUtility.SetDirty(currentES.gameObject);
                    }
#endif
                }
            }
            else
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<EventSystem>();
                AddStandardInputModule(esObj);

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.Undo.RegisterCreatedObjectUndo(esObj, "Create EventSystem");
                }
#endif
            }
        }

        protected void AddStandardInputModule(GameObject esObj)
        {
            System.Type newInputType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (newInputType != null)
            {
                esObj.AddComponent(newInputType);
            }
            else
            {
                esObj.AddComponent<StandaloneInputModule>();
            }
        }
    }
}
