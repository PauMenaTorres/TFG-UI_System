using UnityEngine;
using System;
using System.Collections.Generic;

namespace ModularUIRuntime
{
    [CreateAssetMenu(fileName = "UIConfiguration", menuName = "Modular UI/Config Project")]
    public class UIConfiguration : ScriptableObject
    {
        public enum TargetPlatform
        {
            Desktop,
            MobilePortrait,
            MobileLandscape,
            VR
        }

        public enum GameGenre
        {
            Shooter,
            FPS,
            ActionAdventure,
            RPG,
            MOBA,
            Sandbox,
            Strategy,
            Racing,
            Puzzle,
            Sport,
            Simulator,
            Fighting
        }

        [Serializable]
        public struct GenreThemeMap
        {
            public GameGenre genre;
            public ModularThemeData theme;
        }

        public enum MobileControlMode
        {
            None,
            UIOnly,
            StandardGameplay
        }

        [SerializeField]
        private TargetPlatform _selectedPlatform;

        public TargetPlatform selectedPlatform
        {
            get { return _selectedPlatform; }
            set
            {
                if (_selectedPlatform != value)
                {
                    _selectedPlatform = value;
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.delayCall += ExecuteSwap;
#endif
                }
            }
        }

        public GameGenre selectedGenre;

        [Header("Mobile Settings")]
        public GameObject mobileControlsPrefab;

        [Serializable]
        public struct VRPlatformSettings
        {
            public GameObject vrCameraRigPrefab;
            public GameObject vrEventSystemPrefab;
        }

        [Header("VR Settings")]
        public VRPlatformSettings vrSettings;

        [Header("General Settings")]
        public Vector2 designResolution = new Vector2(800, 600);

        public List<GenreThemeMap> genreThemes = new List<GenreThemeMap>();

        public event Action OnConfigurationChanged;

#if UNITY_EDITOR
        private TargetPlatform _previousPlatform;

        private void ExecuteSwap()
        {
            if (this != null)
            {
                var swapperType = System.Type.GetType("ModularUIRuntime.ModularUIPlatformSwapper, Assembly-CSharp-Editor");
                if (swapperType != null)
                {
                    var method = swapperType.GetMethod("SwapPrefabs", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    if (method != null)
                    {
                        method.Invoke(null, new object[] { _selectedPlatform });
                    }
                }
                OnConfigurationChanged?.Invoke();
            }
        }

        private void OnValidate()
        {
            if (_previousPlatform != _selectedPlatform)
            {
                _previousPlatform = _selectedPlatform;
                UnityEditor.EditorApplication.delayCall -= ExecuteSwap;
                UnityEditor.EditorApplication.delayCall += ExecuteSwap;
            }
            else
            {
                if (_selectedPlatform == TargetPlatform.MobilePortrait)
                {
                    if (designResolution.x > designResolution.y)
                    {
                        float temp = designResolution.x;
                        designResolution.x = designResolution.y;
                        designResolution.y = temp;
                    }
                }
                else
                {
                    if (designResolution.y > designResolution.x)
                    {
                        float temp = designResolution.x;
                        designResolution.x = designResolution.y;
                        designResolution.y = temp;
                    }
                }

                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (this != null)
                    {
                        OnConfigurationChanged?.Invoke();
                    }
                };
            }
        }
#endif

        public ModularThemeData GetActiveTheme()
        {
            foreach (var map in genreThemes)
            {
                if (map.genre == selectedGenre)
                {
                    if (map.theme != null)
                        return map.theme;
                }
            }

            return Resources.Load<ModularThemeData>("DefaultTheme");
        }
    }
}