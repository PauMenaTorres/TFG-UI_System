using UnityEngine;
using System;

namespace ModularUIRuntime
{
    [ExecuteAlways]
    public class ModularThemeManager : MonoBehaviour
    {
        private static bool _isQuitting = false;
        private static ModularThemeManager _instance;

#if UNITY_EDITOR
        static ModularThemeManager()
        {
            UnityEditor.EditorApplication.playModeStateChanged += (state) =>
            {
                if (state == UnityEditor.PlayModeStateChange.EnteredEditMode)
                {
                    _isQuitting = false;
                }
            };
        }
#endif

        public static bool HasInstance => _instance != null;

        public static ModularThemeManager Instance
        {
            get
            {
                if (_isQuitting && Application.isPlaying) return null;

                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<ModularThemeManager>();
                    if (_instance == null && !(_isQuitting && Application.isPlaying))
                    {
                        GameObject go = new GameObject("ModularThemeManager");
                        if (!Application.isPlaying)
                        {
                            go.hideFlags = HideFlags.HideAndDontSave;
                        }
                        _instance = go.AddComponent<ModularThemeManager>();
                        
                        if (Application.isPlaying)
                        {
                            DontDestroyOnLoad(go);
                        }
                    }
                }
                return _instance;
            }
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        [Header("Configuration")]
        [SerializeField] private UIConfiguration config;
        public UIConfiguration Config => config;

        public event Action OnThemeChanged;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                if (Application.isPlaying)
                    Destroy(gameObject);
                else
                    DestroyImmediate(gameObject);
                return;
            }
            _instance = this;

            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }

            InitializeConfig();
        }

        private void OnEnable()
        {
            InitializeConfig();
        }

        private void OnDisable()
        {
            if (config != null)
            {
                config.OnConfigurationChanged -= HandleConfigChanged;
            }
        }

        private void InitializeConfig()
        {
            if (Application.isPlaying)
            {
                config = Resources.Load<UIConfiguration>("UIConfiguration");
            }
            else
            {
                if (config == null)
                {
                    config = Resources.Load<UIConfiguration>("UIConfiguration");
                }

#if UNITY_EDITOR
                if (config == null)
                {
                    config = UnityEditor.AssetDatabase.LoadAssetAtPath<UIConfiguration>("Assets/Plugins/ModularUI/Settings/UIConfiguration.asset");
                }
#endif
            }

            if (config != null)
            {
                config.OnConfigurationChanged -= HandleConfigChanged;
                config.OnConfigurationChanged += HandleConfigChanged;
            }
        }

        private void HandleConfigChanged()
        {
            OnThemeChanged?.Invoke();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                ModularComponents[] components = FindObjectsByType<ModularComponents>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var component in components)
                {
                    component.ApplyThemeInEditor();
                }

                var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null && prefabStage.prefabContentsRoot != null)
                {
                    ModularComponents[] prefabComponents = prefabStage.prefabContentsRoot.GetComponentsInChildren<ModularComponents>(true);
                    foreach (var component in prefabComponents)
                    {
                        component.ApplyThemeInEditor();
                    }
                }
            }
#endif
        }

        public ModularThemeData GetActiveTheme()
        {
            if (config == null)
            {
                InitializeConfig();
            }

            if (config != null)
            {
                return config.GetActiveTheme();
            }

            return null;
        }

        public void SetConfiguration(UIConfiguration newConfig)
        {
            if (config != null)
            {
                config.OnConfigurationChanged -= HandleConfigChanged;
            }

            config = newConfig;

            if (config != null)
            {
                config.OnConfigurationChanged += HandleConfigChanged;
                HandleConfigChanged();
            }
        }
    }
}
