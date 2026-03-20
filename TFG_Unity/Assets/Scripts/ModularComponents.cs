using UnityEngine;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    [ExecuteAlways]
    public abstract class ModularComponents : MonoBehaviour
    {
        [Header("THEME SETTINGS\n")]
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

        protected void ApplyStyle(Image image, ModularStyleBox style)
        {
            if (image == null) return;

            if (style.backgroundType == ModularStyleBox.StyleBoxType.SolidColor)
            {
                image.sprite = null;
                image.color = style.backgroundColor;
            }
            else if (style.backgroundType == ModularStyleBox.StyleBoxType.Sprite)
            {
                image.sprite = style.backgroundSprite;
                image.color = style.tintColor;
            }
            else
            {
                image.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            }
        }
    }
}