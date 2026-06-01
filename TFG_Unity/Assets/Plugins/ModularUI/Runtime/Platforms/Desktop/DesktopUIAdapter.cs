using UnityEngine;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    public class DesktopUIAdapter : BasePlatformUIAdapter
    {
        private readonly Vector2 referenceResolution;
        private readonly float matchWidthOrHeight;

        public DesktopUIAdapter(Vector2 referenceResolution, float matchWidthOrHeight)
        {
            this.referenceResolution = referenceResolution;
            this.matchWidthOrHeight = matchWidthOrHeight;
        }

        public override void SetupCanvas(Canvas targetCanvas)
        {
            CleanupVREnvironment(targetCanvas);

            if (targetCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            if (targetCanvas.transform.localScale != Vector3.one)
            {
                targetCanvas.transform.localScale = Vector3.one;
            }

            CanvasScaler scaler = targetCanvas.GetComponent<CanvasScaler>();

            if (scaler != null)
            {
                if (scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                if (scaler.referenceResolution != referenceResolution)
                    scaler.referenceResolution = referenceResolution;
                if (scaler.screenMatchMode != CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
                    scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                if (scaler.matchWidthOrHeight != matchWidthOrHeight)
                    scaler.matchWidthOrHeight = matchWidthOrHeight;
            }

            if (targetCanvas.GetComponent<GraphicRaycaster>() == null)
            {
                targetCanvas.gameObject.AddComponent<GraphicRaycaster>();
            }
        }
    }
}