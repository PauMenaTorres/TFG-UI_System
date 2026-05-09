using UnityEngine;

namespace ModularUIRuntime
{
    [ExecuteAlways]
    [RequireComponent(typeof(Canvas))]
    public class ModularCanvasInitializer : MonoBehaviour
    {
        public UIConfiguration config;

        private void OnEnable()
        {
            if (config != null)
            {
                config.OnConfigurationChanged += Initialize;
            }

            Initialize();
        }

        private void OnDisable()
        {
            if (config != null)
            {
                config.OnConfigurationChanged -= Initialize;
            }
        }

        private void Initialize()
        {
            if (this == null || config == null)
            {
                return;
            }

            IPlatformFactory factory = new PlatformFactory(config);
            IPlatformUIAdapter adapter = factory.CreateAdapter();

            Canvas canvas = GetComponent<Canvas>();

            if (canvas != null)
            {
                adapter.SetupCanvas(canvas);

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.EditorUtility.SetDirty(gameObject);

                    UnityEngine.UI.CanvasScaler scaler = GetComponent<UnityEngine.UI.CanvasScaler>();

                    if (scaler != null)
                    {
                        UnityEditor.EditorUtility.SetDirty(scaler);
                    }
                }
#endif
            }
        }
    }
}