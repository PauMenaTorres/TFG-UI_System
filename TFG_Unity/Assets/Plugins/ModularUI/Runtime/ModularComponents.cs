using UnityEngine;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    [ExecuteAlways]
    public abstract class ModularComponents : MonoBehaviour
    {
        [Header("THEME SETTINGS")]
        public ModularThemeData currentTheme;
        public bool useOverride = false;

        private bool isApplyingTheme = false;

        protected virtual void Awake()
        {
            ApplyTheme();
        }

        protected virtual void OnEnable()
        {
            if (currentTheme != null)
            {
                currentTheme.OnThemeChanged += ApplyTheme;
            }
        }

        protected virtual void OnDisable()
        {
            if (currentTheme != null)
            {
                currentTheme.OnThemeChanged -= ApplyTheme;
            }
        }

        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return;
            }

            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null && !isApplyingTheme)
                {
                    isApplyingTheme = true;
                    ApplyTheme();
                    isApplyingTheme = false;
                }
            };
#endif
        }

        public virtual void ApplyTheme()
        {
            if (currentTheme == null)
            {
                currentTheme = Resources.Load<ModularThemeData>("DefaultTheme");
            }

            if (currentTheme == null)
            {
                return;
            }
        }

        protected void ApplyStyle(Image image, ModularStyleBox style)
        {
            if (image == null)
            {
                return;
            }

            if (style.backgroundType == ModularStyleBox.StyleBoxType.SolidColor)
            {
                image.sprite = null;
                image.color = style.backgroundColor;
            }

            if (style.backgroundType == ModularStyleBox.StyleBoxType.Sprite)
            {
                image.sprite = style.backgroundSprite;
                image.color = style.tintColor;

                if (image.sprite == null)
                {
                    image.color = new Color(0, 0, 0, 0);
                }
            }

            if (style.backgroundType == ModularStyleBox.StyleBoxType.None)
            {
                image.color = new Color(0, 0, 0, 0);
            }
        }
    }
}