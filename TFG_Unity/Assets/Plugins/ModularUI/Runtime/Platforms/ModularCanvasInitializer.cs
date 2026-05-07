using UnityEngine;

namespace ModularUIRuntime
{
    [RequireComponent(typeof(Canvas))]
    [ExecuteAlways]
    public class ModularCanvasInitializer : MonoBehaviour
    {
        public UIConfiguration config;

        private UIConfiguration lastConfig;

        private void OnEnable()
        {
            UpdateSubscription();
            Refresh();
        }

        private void OnDisable()
        {
            if (config != null)
            {
                config.OnConfigurationChanged -= Refresh;
            }
        }

        private void OnValidate()
        {
            UpdateSubscription();
            Refresh();
        }

        private void UpdateSubscription()
        {
            if (lastConfig != null)
            {
                lastConfig.OnConfigurationChanged -= Refresh;
            }

            if (config != null)
            {
                config.OnConfigurationChanged += Refresh;
            }

            lastConfig = config;
        }

        public void Refresh()
        {
            if (config == null || this == null) return;

#if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this)) return;
#endif

            IPlatformFactory factory = new PlatformFactory(config);
            IPlatformUIAdapter adapter = factory.CreateAdapter();

            adapter.SetupCanvas(GetComponent<Canvas>());
        }
    }
}