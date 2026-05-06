using UnityEngine;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    public class VRUIAdapter : IPlatformUIAdapter
    {
        public void SetupCanvas(Canvas targetCanvas)
        {
            targetCanvas.renderMode = RenderMode.WorldSpace;

            targetCanvas.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

            GraphicRaycaster oldRaycaster = targetCanvas.GetComponent<GraphicRaycaster>();
            if (oldRaycaster != null)
            {
                Object.Destroy(oldRaycaster);
            }
        }
    }
}