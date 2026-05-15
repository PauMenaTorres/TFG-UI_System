using UnityEngine;
using System;

namespace ModularUIRuntime
{
    [ExecuteAlways]
    public class ModularThemeManager : MonoBehaviour
    {
        private static bool _isQuitting = false;
        private static ModularThemeManager _instance;

        public static bool HasInstance => _instance != null;

        public static ModularThemeManager Instance
        {
            get
            {
                if (_isQuitting) return null;

                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<ModularThemeManager>();
                    if (_instance == null && !_isQuitting)
                    {
                        GameObject go = new GameObject("ModularThemeManager");
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
            if (config == null)
            {
                config = Resources.Load<UIConfiguration>("UIConfiguration");
            }

            if (config != null)
            {
                // Remove existing listener to avoid duplicates
                config.OnConfigurationChanged -= HandleConfigChanged;
                config.OnConfigurationChanged += HandleConfigChanged;
            }
        }

        private void HandleConfigChanged()
        {
            OnThemeChanged?.Invoke();
            
            // Notify all components in the scene to update
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                ModularComponents[] components = FindObjectsByType<ModularComponents>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var component in components)
                {
                    component.ApplyTheme();
                    UnityEditor.EditorUtility.SetDirty(component);
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
