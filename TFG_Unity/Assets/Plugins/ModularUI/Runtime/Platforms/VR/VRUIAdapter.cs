using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace ModularUIRuntime
{
    public class VRUIAdapter : BasePlatformUIAdapter
    {
        private readonly UIConfiguration.VRPlatformSettings vrSettings;

        public VRUIAdapter(UIConfiguration.VRPlatformSettings settings)
        {
            this.vrSettings = settings;
        }

        private System.Type FindType(string typeName)
        {
            var type = System.Type.GetType(typeName);
            if (type != null) return type;

            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName);
                if (type != null) return type;
            }
            return null;
        }

        private System.Reflection.FieldInfo GetFieldIncludingBaseTypes(System.Type type, string fieldName, System.Reflection.BindingFlags bindingFlags)
        {
            System.Type currentType = type;
            while (currentType != null)
            {
                var field = currentType.GetField(fieldName, bindingFlags);
                if (field != null) return field;
                currentType = currentType.BaseType;
            }
            return null;
        }

        private void InvokeMethod(object target, string methodName, object[] parameters)
        {
            if (target == null) return;
            var method = target.GetType().GetMethod(methodName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (method != null)
            {
                method.Invoke(target, parameters);
            }
            else
            {
                foreach (var m in target.GetType().GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                {
                    if (m.Name.Equals(methodName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        m.Invoke(target, parameters);
                        return;
                    }
                }
            }
        }

        private void SetFieldOrProperty(object target, string name, object value)
        {
            if (target == null) return;
            System.Type type = target.GetType();
            
            var prop = type.GetProperty(name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (prop == null)
            {
                foreach (var p in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                {
                    if (p.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                    {
                        prop = p;
                        break;
                    }
                }
            }

            if (prop != null)
            {
                try
                {
                    var getMethod = prop.GetGetMethod(true);
                    if (getMethod != null)
                    {
                        var currentValue = getMethod.Invoke(target, null);
                        if (object.Equals(currentValue, value))
                        {
                            return; // Already equal, skip setting to avoid dirtying
                        }
                    }
                }
                catch {}

                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setMethod.Invoke(target, new object[] { value });
                    return;
                }
            }

            var field = GetFieldIncludingBaseTypes(type, name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field == null)
            {
                System.Type currentType = type;
                while (currentType != null)
                {
                    foreach (var f in currentType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                    {
                        if (f.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                        {
                            field = f;
                            break;
                        }
                    }
                    if (field != null) break;
                    currentType = currentType.BaseType;
                }
            }

            if (field != null)
            {
                try
                {
                    var currentValue = field.GetValue(target);
                    if (object.Equals(currentValue, value))
                    {
                        return; // Already equal, skip setting to avoid dirtying
                    }
                }
                catch {}

                field.SetValue(target, value);
            }
        }

        public override void SetupCanvas(Canvas targetCanvas)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && UnityEditor.EditorUtility.IsPersistent(targetCanvas.gameObject))
            {
                return;
            }
#endif
            if (targetCanvas.renderMode != RenderMode.WorldSpace)
            {
                targetCanvas.renderMode = RenderMode.WorldSpace;
            }
            Vector3 vrScale = new Vector3(0.002f, 0.002f, 1f);
            if (targetCanvas.transform.localScale != vrScale)
            {
                targetCanvas.transform.localScale = vrScale;
            }
            Vector3 vrPos = new Vector3(0f, 1.5f, 0f);
            if (targetCanvas.transform.localPosition != vrPos)
            {
                targetCanvas.transform.localPosition = vrPos;
            }

            // Configure CanvasScaler
            CanvasScaler scaler = targetCanvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = targetCanvas.gameObject.AddComponent<CanvasScaler>();
            }
            if (scaler != null)
            {
                if (scaler.referencePixelsPerUnit != 100f)
                    scaler.referencePixelsPerUnit = 100f;
                if (scaler.dynamicPixelsPerUnit != 1f)
                    scaler.dynamicPixelsPerUnit = 1f;
            }

            // Configure GraphicRaycaster (should be kept for interaction)
            GraphicRaycaster raycaster = targetCanvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                raycaster = targetCanvas.gameObject.AddComponent<GraphicRaycaster>();
            }
            if (raycaster != null)
            {
                if (raycaster.ignoreReversedGraphics != true)
                    raycaster.ignoreReversedGraphics = true;
                if (raycaster.blockingObjects != GraphicRaycaster.BlockingObjects.None)
                    raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
                if (raycaster.blockingMask != -1)
                    raycaster.blockingMask = -1; // Everything
            }

            // Configure OVR Overlay Canvas
            System.Type ovrOverlayCanvasType = FindType("OVROverlayCanvas") ?? FindType("Oculus.Interaction.OVROverlayCanvas");
            if (ovrOverlayCanvasType != null && targetCanvas.GetComponent(ovrOverlayCanvasType) == null)
            {
                targetCanvas.gameObject.AddComponent(ovrOverlayCanvasType);
            }

            // Setup Oculus SDK Pointable/Interaction child objects
            System.Type pointableCanvasType = FindType("Oculus.Interaction.PointableCanvas") ?? FindType("PointableCanvas");
            System.Type rayInteractableType = FindType("Oculus.Interaction.RayInteractable") ?? FindType("RayInteractable");
            System.Type pokeInteractableType = FindType("Oculus.Interaction.PokeInteractable") ?? FindType("PokeInteractable");

#if UNITY_EDITOR
            bool isPrefabInstance = !Application.isPlaying && UnityEditor.PrefabUtility.IsPartOfPrefabInstance(targetCanvas.gameObject);
            if (isPrefabInstance)
            {
                // Clean up any orphaned interaction objects in the scene first to avoid leaks
                var allGOs = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var go in allGOs)
                {
                    if (go != null && (go.name == "ISDK_RayCanvasInteraction" || go.name == "ISDK_PokeCanvasInteraction"))
                    {
                        if (go.transform.parent == null)
                        {
                            Object.DestroyImmediate(go, true);
                        }
                    }
                }
                
                SetupVREnvironment(targetCanvas);
                return;
            }
#endif

            if (pointableCanvasType != null)
            {
                // 1. Ray Canvas Interaction
                Transform rayInteractionTrans = targetCanvas.transform.Find("ISDK_RayCanvasInteraction");
                GameObject rayInteractionGo;
                if (rayInteractionTrans == null)
                {
                    rayInteractionGo = new GameObject("ISDK_RayCanvasInteraction", typeof(RectTransform));
                    rayInteractionGo.transform.SetParent(targetCanvas.transform, false);
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        UnityEditor.Undo.RegisterCreatedObjectUndo(rayInteractionGo, "Create Ray Interaction Child");
                    }
#endif
                }
                else
                {
                    rayInteractionGo = rayInteractionTrans.gameObject;
                }

                if (rayInteractableType != null)
                {
                    SetupPointableCanvas(rayInteractionGo, targetCanvas, pointableCanvasType, rayInteractableType);
                }

                // 2. Poke Canvas Interaction
                Transform pokeInteractionTrans = targetCanvas.transform.Find("ISDK_PokeCanvasInteraction");
                GameObject pokeInteractionGo;
                if (pokeInteractionTrans == null)
                {
                    pokeInteractionGo = new GameObject("ISDK_PokeCanvasInteraction", typeof(RectTransform));
                    pokeInteractionGo.transform.SetParent(targetCanvas.transform, false);
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        UnityEditor.Undo.RegisterCreatedObjectUndo(pokeInteractionGo, "Create Poke Interaction Child");
                    }
#endif
                }
                else
                {
                    pokeInteractionGo = pokeInteractionTrans.gameObject;
                }

                if (pokeInteractableType != null)
                {
                    SetupPointableCanvas(pokeInteractionGo, targetCanvas, pointableCanvasType, pokeInteractableType);
                }
            }

            SetupVREnvironment(targetCanvas);
        }

        private void SetupPointableCanvas(GameObject go, Canvas canvas, System.Type pointableCanvasType, System.Type interactableType)
        {
            Component pointableCanvas = go.GetComponent(pointableCanvasType);
            if (pointableCanvas == null)
            {
                pointableCanvas = go.AddComponent(pointableCanvasType);
            }

            // 1. Set canvas on PointableCanvas
            // Try public InjectCanvas method
            InvokeMethod(pointableCanvas, "InjectCanvas", new object[] { canvas });
            InvokeMethod(pointableCanvas, "InjectAllPointableCanvas", new object[] { canvas });
            // Direct property/field settings
            SetFieldOrProperty(pointableCanvas, "Canvas", canvas);
            SetFieldOrProperty(pointableCanvas, "_canvas", canvas);

            // Setup interactable component
            Component interactable = go.GetComponent(interactableType);
            if (interactable == null)
            {
                interactable = go.AddComponent(interactableType);
            }

            // 2. Set PointableElement on the interactable
            // Try public InjectOptionalPointableElement method
            InvokeMethod(interactable, "InjectOptionalPointableElement", new object[] { pointableCanvas });
            // Direct property/field settings
            SetFieldOrProperty(interactable, "PointableElement", pointableCanvas);
            SetFieldOrProperty(interactable, "_pointableElement", pointableCanvas);

            // Setup child Surface GameObject for Oculus Raycast/Poke interaction
            System.Type planeSurfaceType = FindType("Oculus.Interaction.Surfaces.PlaneSurface") ?? FindType("PlaneSurface");
            System.Type clippedPlaneSurfaceType = FindType("Oculus.Interaction.Surfaces.ClippedPlaneSurface") ?? FindType("ClippedPlaneSurface");
            System.Type boundsClipperType = FindType("Oculus.Interaction.Surfaces.BoundsClipper") ?? FindType("BoundsClipper");

            if (planeSurfaceType != null && clippedPlaneSurfaceType != null && boundsClipperType != null)
            {
                Transform surfaceTrans = go.transform.Find("Surface");
                GameObject surfaceGo;
                if (surfaceTrans == null)
                {
                    surfaceGo = new GameObject("Surface", typeof(RectTransform));
                    surfaceGo.transform.SetParent(go.transform, false);
                    
                    RectTransform surfaceRect = surfaceGo.GetComponent<RectTransform>();
                    if (surfaceRect != null)
                    {
                        surfaceRect.anchorMin = Vector2.zero;
                        surfaceRect.anchorMax = Vector2.one;
                        surfaceRect.anchoredPosition = Vector2.zero;
                        surfaceRect.sizeDelta = Vector2.zero;
                    }
                }
                else
                {
                    surfaceGo = surfaceTrans.gameObject;
                }

                Component planeSurface = surfaceGo.GetComponent(planeSurfaceType);
                if (planeSurface == null)
                {
                    planeSurface = surfaceGo.AddComponent(planeSurfaceType);
                }

                Component boundsClipper = surfaceGo.GetComponent(boundsClipperType);
                if (boundsClipper == null)
                {
                    boundsClipper = surfaceGo.AddComponent(boundsClipperType);
                }

                if (boundsClipper != null)
                {
                    RectTransform canvasRect = canvas.GetComponent<RectTransform>();
                    Vector2 size = canvasRect != null ? canvasRect.sizeDelta : new Vector2(500, 250);

                    SetFieldOrProperty(boundsClipper, "Position", Vector3.zero);
                    SetFieldOrProperty(boundsClipper, "Size", new Vector3(size.x, size.y, 0.01f));
                }

                Component clippedPlaneSurface = surfaceGo.GetComponent(clippedPlaneSurfaceType);
                if (clippedPlaneSurface == null)
                {
                    clippedPlaneSurface = surfaceGo.AddComponent(clippedPlaneSurfaceType);
                }

                if (clippedPlaneSurface != null && planeSurface != null && boundsClipper != null)
                {
                    // Inject PlaneSurface
                    InvokeMethod(clippedPlaneSurface, "InjectPlaneSurface", new object[] { planeSurface });
                    SetFieldOrProperty(clippedPlaneSurface, "PlaneSurface", planeSurface);
                    SetFieldOrProperty(clippedPlaneSurface, "_planeSurface", planeSurface);

                    // Inject Clippers
                    System.Type boundsClipperInterfaceType = FindType("Oculus.Interaction.Surfaces.IBoundsClipper") ?? FindType("IBoundsClipper");
                    if (boundsClipperInterfaceType != null)
                    {
                        var listType = typeof(List<>).MakeGenericType(boundsClipperInterfaceType);
                        var list = System.Activator.CreateInstance(listType);
                        var addMethod = listType.GetMethod("Add");
                        if (addMethod != null)
                        {
                            addMethod.Invoke(list, new object[] { boundsClipper });
                        }
                        InvokeMethod(clippedPlaneSurface, "InjectClippers", new object[] { list });

                        // Safe property and field setting with correct types
                        var objList = new List<UnityEngine.Object> { boundsClipper as UnityEngine.Object };
                        SetFieldOrProperty(clippedPlaneSurface, "_clippers", objList);
                        SetFieldOrProperty(clippedPlaneSurface, "Clippers", list);
                    }
                }

                // Set surfaces on interactables
                if (interactableType.Name.Contains("RayInteractable"))
                {
                    // Inject Surfaces
                    InvokeMethod(interactable, "InjectSurface", new object[] { clippedPlaneSurface });
                    InvokeMethod(interactable, "InjectAllRayInteractable", new object[] { clippedPlaneSurface });
                    InvokeMethod(interactable, "InjectOptionalSelectSurface", new object[] { planeSurface });

                    // Set fields/properties directly as fallback/additional safety
                    SetFieldOrProperty(interactable, "Surface", clippedPlaneSurface);
                    SetFieldOrProperty(interactable, "_surface", clippedPlaneSurface);
                    SetFieldOrProperty(interactable, "SelectSurface", planeSurface);
                    SetFieldOrProperty(interactable, "_selectSurface", planeSurface);
                }
                else if (interactableType.Name.Contains("PokeInteractable"))
                {
                    // Inject Surfaces
                    InvokeMethod(interactable, "InjectSurfacePatch", new object[] { clippedPlaneSurface });
                    InvokeMethod(interactable, "InjectAllPokeInteractable", new object[] { clippedPlaneSurface });

                    // Set fields/properties directly as fallback/additional safety
                    SetFieldOrProperty(interactable, "SurfacePatch", clippedPlaneSurface);
                    SetFieldOrProperty(interactable, "_surfacePatch", clippedPlaneSurface);
                }
            }

            // Add LayoutElement and ignore layout
            LayoutElement layoutElement = go.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = go.AddComponent<LayoutElement>();
            }
            if (layoutElement != null)
            {
                layoutElement.ignoreLayout = true;
            }
        }

        private void SetupVREnvironment(Canvas canvas)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying && IsPrefabStageOrAsset(canvas.gameObject))
            {
                return;
            }
#endif
            Camera vrCamera = null;

            Camera[] allCameras = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (Camera cam in allCameras)
            {
                if (IsVRCamera(cam))
                {
                    vrCamera = cam;
                    break;
                }
            }

            if (vrCamera == null && vrSettings.vrCameraRigPrefab != null)
            {
                GameObject rigInstance = null;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    rigInstance = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(vrSettings.vrCameraRigPrefab);
                    if (rigInstance != null)
                    {
                        UnityEditor.Undo.RegisterCreatedObjectUndo(rigInstance, "Create VR Rig");
                    }
                }
                else
                {
                    rigInstance = Object.Instantiate(vrSettings.vrCameraRigPrefab);
                }
#else
                rigInstance = Object.Instantiate(vrSettings.vrCameraRigPrefab);
#endif
                if (rigInstance != null)
                {
                    vrCamera = rigInstance.GetComponentInChildren<Camera>();
                }
            }

            allCameras = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (Camera cam in allCameras)
            {
                if (vrCamera != null && cam.transform.root == vrCamera.transform.root)
                {
                    continue;
                }

                if (IsVRCamera(cam))
                {
                    continue;
                }

                if (Application.isPlaying) Object.Destroy(cam.gameObject);
                else Object.DestroyImmediate(cam.gameObject, true);
            }

            if (vrCamera != null && canvas.worldCamera != vrCamera)
            {
                canvas.worldCamera = vrCamera;
            }
            
            EventSystem currentES = Object.FindFirstObjectByType<EventSystem>();
            bool needsNewES = false;

            if (currentES != null)
            {
                bool isVREventSystem = false;
                if (vrSettings.vrEventSystemPrefab != null)
                {
#if UNITY_EDITOR
                    var prefabSource = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(currentES.gameObject);
                    if (prefabSource == vrSettings.vrEventSystemPrefab)
                    {
                        isVREventSystem = true;
                    }
#endif
                    if (currentES.name.StartsWith(vrSettings.vrEventSystemPrefab.name))
                    {
                        isVREventSystem = true;
                    }
                }

                if (!isVREventSystem)
                {
                    foreach (var comp in currentES.GetComponents<Component>())
                    {
                        if (comp == null) continue;
                        string typeName = comp.GetType().FullName;
                        if (string.IsNullOrEmpty(typeName)) continue;
                        if (typeName.Contains("OVRInput") || typeName.Contains("PointableCanvasModule"))
                        {
                            isVREventSystem = true;
                            break;
                        }
                    }
                }

                if (!isVREventSystem)
                {
                    if (Application.isPlaying) Object.Destroy(currentES.gameObject);
                    else Object.DestroyImmediate(currentES.gameObject, true);
                    needsNewES = true;
                }
            }
            else
            {
                needsNewES = true;
            }

            if (needsNewES && vrSettings.vrEventSystemPrefab != null)
            {
                GameObject esInstance = null;
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    esInstance = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(vrSettings.vrEventSystemPrefab);
                    if (esInstance != null)
                    {
                        UnityEditor.Undo.RegisterCreatedObjectUndo(esInstance, "Create VR EventSystem");
                    }
                }
                else
                {
                    esInstance = Object.Instantiate(vrSettings.vrEventSystemPrefab);
                }
#else
                esInstance = Object.Instantiate(vrSettings.vrEventSystemPrefab);
#endif
                if (esInstance != null)
                {
                    currentES = esInstance.GetComponent<EventSystem>();
                }
            }

            if (canvas.GetComponent("OVRRaycaster") == null)
            {
                System.Type ovrRaycasterType = System.Type.GetType("OVRRaycaster, Assembly-CSharp");
                if (ovrRaycasterType != null)
                {
                    canvas.gameObject.AddComponent(ovrRaycasterType);
                }
            }

            // Setup Oculus PointableCanvasModule on the EventSystem
            System.Type pointableCanvasModuleType = FindType("Oculus.Interaction.PointableCanvasModule") 
                                                    ?? FindType("PointableCanvasModule");
            if (pointableCanvasModuleType != null && currentES != null)
            {
                if (currentES.GetComponent(pointableCanvasModuleType) == null)
                {
                    currentES.gameObject.AddComponent(pointableCanvasModuleType);
                }
            }
        }
    }
}