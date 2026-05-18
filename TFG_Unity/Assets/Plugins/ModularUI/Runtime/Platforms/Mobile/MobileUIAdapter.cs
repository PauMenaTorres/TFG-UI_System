using UnityEngine;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    public class MobileUIAdapter : BasePlatformUIAdapter
    {
        private readonly Vector2 referenceResolution;
        private readonly float matchWidthOrHeight;

        public MobileUIAdapter(Vector2 referenceResolution, float matchWidthOrHeight)
        {
            this.referenceResolution = referenceResolution;
            this.matchWidthOrHeight = matchWidthOrHeight;
        }

        public override void SetupCanvas(Canvas targetCanvas)
        {
            CleanupVREnvironment(targetCanvas);

            targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            targetCanvas.transform.localScale = Vector3.one;

            CanvasScaler scaler = targetCanvas.GetComponent<CanvasScaler>();

            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = referenceResolution;
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = matchWidthOrHeight;
            }

            if (targetCanvas.GetComponent<GraphicRaycaster>() == null)
            {
                targetCanvas.gameObject.AddComponent<GraphicRaycaster>();
            }

            if (Application.isPlaying)
            {
                HandleAdaptiveControls(targetCanvas);
            }
        }

        private void HandleAdaptiveControls(Canvas canvas)
        {
            MobileControlRequirement requirement = Object.FindObjectOfType<MobileControlRequirement>();
            
            if (requirement == null || requirement.mode == UIConfiguration.MobileControlMode.None)
            {
                return;
            }

            GameObject prefabToSpawn = requirement.customControlsPrefab != null 
                ? requirement.customControlsPrefab 
                : GetConfig()?.mobileControlsPrefab;

            if (prefabToSpawn != null)
            {
                if (canvas.transform.Find(prefabToSpawn.name) != null) return;

                GameObject controls = Object.Instantiate(prefabToSpawn, canvas.transform);
                controls.name = prefabToSpawn.name;
                
                MobileTouchInput input = controls.GetComponent<MobileTouchInput>();
                if (input != null)
                {
                    input.EnableInput();
                }
            }
        }

        private UIConfiguration GetConfig()
        {
            return Object.FindObjectOfType<ModularCanvasInitializer>()?.config;
        }
    }
}