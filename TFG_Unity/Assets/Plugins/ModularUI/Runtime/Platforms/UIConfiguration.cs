using UnityEngine;
using System;

namespace ModularUIRuntime
{
    [CreateAssetMenu(fileName = "UIConfiguration", menuName = "Modular UI/Config Project")]
    public class UIConfiguration : ScriptableObject
    {
        public enum TargetPlatform 
        {   
            Desktop,
            Mobile,
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

        public TargetPlatform selectedPlatform;
        public GameGenre selectedGenre;

        public event Action OnConfigurationChanged;

        private void OnValidate()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null)
                    OnConfigurationChanged?.Invoke();
            };
#endif
        }
    }
}