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

        public TargetPlatform selectedPlatform;
        public GameGenre selectedGenre;

        public List<GenreThemeMap> genreThemes = new List<GenreThemeMap>();

        public event Action OnConfigurationChanged;

        private void OnValidate()
        {
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
                    return map.theme;
                }
            }

            return null;
        }
    }
}