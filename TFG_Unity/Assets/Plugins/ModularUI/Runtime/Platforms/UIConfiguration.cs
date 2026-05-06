using UnityEngine;

namespace ModularUIRuntime
{
    [CreateAssetMenu(fileName = "UIConfiguration", menuName = "Modular UI/Config")]
    public class UIConfiguration : ScriptableObject
    {
        public enum TargetPlatform 
        {   
            Desktop,
            Mobile,
            VR 
        }
        public TargetPlatform selectedPlatform;
    }
}