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
        }
    }
}