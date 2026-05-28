using UnityEngine;

namespace ModularUIRuntime
{
    [ExecuteAlways]
    [RequireComponent(typeof(Canvas))]
    public class ModularCanvasInitializer : MonoBehaviour
    {
        public UIConfiguration config;

        [Header("Editor Settings")]
        public bool autoAdaptInEditor = false;

        private void OnEnable()
        {
            if (config != null)
            {
                config.OnConfigurationChanged -= Initialize;
                config.OnConfigurationChanged += Initialize;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying && !autoAdaptInEditor)
            {
                return;
            }
#endif

            Initialize();
        }

        private void OnDisable()
        {
            if (config != null)
            {
                config.OnConfigurationChanged -= Initialize;
            }
        }

        public void ForceInitialize()
        {
            InitializeInternal(true);
        }

        private void Initialize()
        {
            InitializeInternal(false);
        }

        private void InitializeInternal(bool force)
        {
            if (this == null || config == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying && !autoAdaptInEditor && !force)
            {
                return;
            }
#endif

            IPlatformFactory factory = new PlatformFactory(config);
            IPlatformUIAdapter adapter = factory.CreateAdapter();

            Canvas canvas = GetComponent<Canvas>();

            if (canvas == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEngine.UI.CanvasScaler scaler = GetComponent<UnityEngine.UI.CanvasScaler>();

                int canvasHash = GetCanvasHash(canvas, scaler);
                adapter.SetupCanvas(canvas);
                int newHash = GetCanvasHash(canvas, scaler);

                if (canvasHash != newHash)
                {
                    UnityEditor.EditorUtility.SetDirty(gameObject);

                    if (scaler != null)
                    {
                        UnityEditor.EditorUtility.SetDirty(scaler);
                    }
                }

                return;
            }
#endif

            adapter.SetupCanvas(canvas);
        }

#if UNITY_EDITOR
        private int GetCanvasHash(Canvas canvas, UnityEngine.UI.CanvasScaler scaler)
        {
            int hash = canvas.renderMode.GetHashCode();
            hash = hash * 31 + canvas.transform.localScale.GetHashCode();

            if (scaler != null)
            {
                hash = hash * 31 + scaler.uiScaleMode.GetHashCode();
                hash = hash * 31 + scaler.referenceResolution.GetHashCode();
                hash = hash * 31 + scaler.screenMatchMode.GetHashCode();
                hash = hash * 31 + scaler.matchWidthOrHeight.GetHashCode();
            }

            return hash;
        }
#endif
    }
}