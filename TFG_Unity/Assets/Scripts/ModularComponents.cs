using UnityEngine;

namespace ModularUIRuntime
{
    [ExecuteAlways]
    public abstract class ModularComponents : MonoBehaviour
    {
        [Header("Theme Settings")]
        public ModularThemeData currentTheme;

        [Tooltip("If is activated this object will igonre the global theme")]
        public bool useOverride = false;

        protected virtual void Awake()
        {
            ApplyTheme();
        }

        protected virtual void OnEnable()
        {
            currentTheme.OnThemeChanged += ApplyTheme;
        }

        protected virtual void OnDisable()
        {
            currentTheme.OnThemeChanged -= ApplyTheme;
        }

        protected virtual void OnValidate()
        {
            ApplyTheme();
        }

        public virtual void ApplyTheme()
        {
            if (currentTheme == null)
            {
                currentTheme = Resources.Load<ModularThemeData>("DefaultTheme");
            }

            if (currentTheme == null || useOverride)
            {
                return;
            }
        }
    }
}