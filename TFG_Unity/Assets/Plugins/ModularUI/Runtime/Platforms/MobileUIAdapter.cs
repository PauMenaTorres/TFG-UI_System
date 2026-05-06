using UnityEngine;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    public class MobileUIAdapter : IPlatformUIAdapter
    {
        public void SetupCanvas(Canvas targetCanvas)
        {
            targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = targetCanvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }

            if (targetCanvas.GetComponent<GraphicRaycaster>() == null)
            {
                targetCanvas.gameObject.AddComponent<GraphicRaycaster>();
            }
        }
    }
}
