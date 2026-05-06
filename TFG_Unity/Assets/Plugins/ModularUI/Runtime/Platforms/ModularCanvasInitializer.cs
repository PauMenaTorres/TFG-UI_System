using UnityEngine;

namespace ModularUIRuntime
{
    [RequireComponent(typeof(Canvas))]
    public class ModularCanvasInitializer : MonoBehaviour
    {
        public UIConfiguration config;

        private void Awake()
        {
            IPlatformFactory factory = new PlatformFactory(config);
            IPlatformUIAdapter adapter = factory.CreateAdapter();

            adapter.SetupCanvas(GetComponent<Canvas>());
        }
    }
}