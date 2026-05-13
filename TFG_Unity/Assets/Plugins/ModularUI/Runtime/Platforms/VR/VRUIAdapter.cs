using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ModularUIRuntime
{
    public class VRUIAdapter : BasePlatformUIAdapter
    {
        private readonly UIConfiguration.VRPlatformSettings vrSettings;

        public VRUIAdapter(UIConfiguration.VRPlatformSettings settings)
        {
            this.vrSettings = settings;
        }

        public override void SetupCanvas(Canvas targetCanvas)
        {
            targetCanvas.renderMode = RenderMode.WorldSpace;
            targetCanvas.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

            GraphicRaycaster oldRaycaster = targetCanvas.GetComponent<GraphicRaycaster>();
            if (oldRaycaster != null)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(oldRaycaster);
                }
                else
                {
                    Object.DestroyImmediate(oldRaycaster, true);
                }
            }

            SetupVREnvironment(targetCanvas);
        }

        private void SetupVREnvironment(Canvas canvas)
        {
            Camera vrCamera = null;

            Camera[] allCameras = Object.FindObjectsOfType<Camera>();
            foreach (Camera cam in allCameras)
            {
                if (cam.transform.root.name.Contains("OVRCameraRig") || cam.transform.root.name.Contains("VR"))
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

            allCameras = Object.FindObjectsOfType<Camera>();
            foreach (Camera cam in allCameras)
            {
                if (vrCamera != null && cam.transform.root == vrCamera.transform.root)
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
            
            EventSystem currentES = Object.FindObjectOfType<EventSystem>();
            bool needsNewES = false;

            if (currentES != null)
            {
                if (currentES.GetComponent("OVRInputModule") == null)
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
            }

            if (canvas.GetComponent("OVRRaycaster") == null)
            {
                System.Type ovrRaycasterType = System.Type.GetType("OVRRaycaster, Assembly-CSharp");
                if (ovrRaycasterType != null)
                {
                    canvas.gameObject.AddComponent(ovrRaycasterType);
                }
            }
        }
    }
}