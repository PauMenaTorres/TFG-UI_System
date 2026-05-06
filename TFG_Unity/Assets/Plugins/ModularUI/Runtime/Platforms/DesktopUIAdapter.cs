using UnityEngine;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    public class DesktopUIAdapter : IPlatformUIAdapter
    {
        public void SetupCanvas(Canvas targetCanvas)
        {
            targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            if (targetCanvas.GetComponent<GraphicRaycaster>() == null)
            {
                targetCanvas.gameObject.AddComponent<GraphicRaycaster>();
            }
        }
    }
}
