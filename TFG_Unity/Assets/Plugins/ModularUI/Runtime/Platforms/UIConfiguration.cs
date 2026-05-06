using UnityEngine;

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
    }
}