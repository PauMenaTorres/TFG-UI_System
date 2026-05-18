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

        public TargetPlatform selectedPlatform;
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

        private void OnValidate()
        {
            if (selectedPlatform == TargetPlatform.MobilePortrait)
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

#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null)
                {
                    OnConfigurationChanged?.Invoke();
                }
            };
#endif
        }

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