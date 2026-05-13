using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ModularUIRuntime
{
    public abstract class BasePlatformUIAdapter : IPlatformUIAdapter
    {
        public abstract void SetupCanvas(Canvas targetCanvas);

        protected void CleanupVREnvironment(Canvas targetCanvas)
        {
            Component ovrRaycaster = targetCanvas.GetComponent("OVRRaycaster");
            if (ovrRaycaster != null)
            {
                if (Application.isPlaying) Object.Destroy(ovrRaycaster);
                else Object.DestroyImmediate(ovrRaycaster, true);
            }

            if (targetCanvas.worldCamera != null && (targetCanvas.worldCamera.transform.root.name.Contains("OVRCameraRig") || targetCanvas.worldCamera.transform.root.name.Contains("VR")))
            {
                targetCanvas.worldCamera = null;
            }
            
            Camera[] allCameras = Object.FindObjectsOfType<Camera>();
            foreach (Camera cam in allCameras)
            {
                if (cam.transform.root.name.Contains("OVRCameraRig") || cam.transform.root.name.Contains("VR"))
                {
                    if (Application.isPlaying) Object.Destroy(cam.transform.root.gameObject);
                    else Object.DestroyImmediate(cam.transform.root.gameObject, true);
                    break;
                }
            }

            Camera mainCam = Object.FindObjectOfType<Camera>();
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
                camObj.transform.position = new Vector3(0, 1, -10);
            }

            EnsureStandardEventSystem();
        }

        protected void EnsureStandardEventSystem()
        {
            EventSystem currentES = Object.FindObjectOfType<EventSystem>();
            
            if (currentES != null)
            {
                Component ovrInput = currentES.GetComponent("OVRInputModule");
                if (ovrInput != null)
                {
                    if (Application.isPlaying) Object.Destroy(ovrInput);
                    else Object.DestroyImmediate(ovrInput, true);
                }

                if (currentES.GetComponent<BaseInputModule>() == null)
                {
                    AddStandardInputModule(currentES.gameObject);
                }
            }
            else
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<EventSystem>();
                AddStandardInputModule(esObj);
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
