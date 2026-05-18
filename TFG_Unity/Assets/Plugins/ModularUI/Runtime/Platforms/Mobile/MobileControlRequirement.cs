using UnityEngine;

namespace ModularUIRuntime
{
    [AddComponentMenu("Modular UI/Mobile/Mobile Control Requirement")]
    public class MobileControlRequirement : MonoBehaviour
    {
        public UIConfiguration.MobileControlMode mode = UIConfiguration.MobileControlMode.StandardGameplay;

        [Header("Optional Overrides")]
        public GameObject customControlsPrefab;

        public static MobileControlRequirement Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}
