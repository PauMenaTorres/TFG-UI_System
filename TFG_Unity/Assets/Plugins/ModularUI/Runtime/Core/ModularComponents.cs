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
        protected bool shouldMarkDirty = false;

        protected virtual void Awake()
        {
            ApplyTheme();
        }

        protected virtual void OnEnable()
        {
            #if UNITY_EDITOR
            if (UnityEditor.EditorUtility.IsPersistent(this))
            {
                return;
            }
            #endif

            // Unsubscribe first to avoid duplicate events
            if (currentTheme != null)
            {
                currentTheme.OnThemeChanged -= HandleThemeChanged;
            }

            ApplyTheme();

            if (currentTheme != null)
            {
                currentTheme.OnThemeChanged -= HandleThemeChanged;
                currentTheme.OnThemeChanged += HandleThemeChanged;
            }

            // Subscribe to global theme changes
            if (ModularThemeManager.Instance != null)
            {
                ModularThemeManager.Instance.OnThemeChanged -= HandleThemeChanged;
                ModularThemeManager.Instance.OnThemeChanged += HandleThemeChanged;
            }
        }

        protected virtual void OnDisable()
        {
            #if UNITY_EDITOR
            if (UnityEditor.EditorUtility.IsPersistent(this))
            {
                return;
            }
            #endif

            if (currentTheme != null)
            {
                currentTheme.OnThemeChanged -= HandleThemeChanged;
            }

            if (ModularThemeManager.HasInstance && ModularThemeManager.Instance != null)
            {
                ModularThemeManager.Instance.OnThemeChanged -= HandleThemeChanged;
            }
        }

        private void HandleThemeChanged()
        {
            #if UNITY_EDITOR
                shouldMarkDirty = true;
            #endif

                ApplyTheme();

            #if UNITY_EDITOR
                shouldMarkDirty = false;
            #endif
        }

        protected virtual void OnValidate()
        {
            #if UNITY_EDITOR
                if (UnityEditor.EditorUtility.IsPersistent(this))
                {
                    return;
                }

                if (Application.isPlaying)
                {
                    return;
                }

                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (this != null && !isApplyingTheme)
                    {
                        isApplyingTheme = true;
                        shouldMarkDirty = true;

                        ApplyTheme();

                        shouldMarkDirty = false;
                        isApplyingTheme = false;
                    }
                };
            #endif
        }

        private static ModularThemeData cachedDefaultTheme;

        public virtual void ApplyTheme()
        {
            #if UNITY_EDITOR
            if (UnityEditor.EditorUtility.IsPersistent(this))
            {
                return;
            }
            #endif

            if (!useOverride)
            {
                ModularThemeManager manager = ModularThemeManager.Instance;
                if (manager != null)
                {
                    ModularThemeData activeTheme = manager.GetActiveTheme();
                    if (activeTheme != null)
                    {
                        // If the theme changed, unsubscribe from old and subscribe to new
                        if (currentTheme != activeTheme)
                        {
                            if (currentTheme != null) currentTheme.OnThemeChanged -= HandleThemeChanged;
                            currentTheme = activeTheme;
                            if (currentTheme != null) currentTheme.OnThemeChanged += HandleThemeChanged;
                        }
                    }
                }
            }

            if (currentTheme == null)
            {
                if (cachedDefaultTheme == null)
                {
                    cachedDefaultTheme = Resources.Load<ModularThemeData>("DefaultTheme");
                }
                currentTheme = cachedDefaultTheme;
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

            bool changed = false;

            if (style.backgroundType == ModularStyleBox.StyleBoxType.SolidColor)
            {
                if (image.sprite != null)
                {
                    image.sprite = null;
                    changed = true;
                }
                if (image.color != style.backgroundColor)
                {
                    image.color = style.backgroundColor;
                    changed = true;
                }
            }
            else if (style.backgroundType == ModularStyleBox.StyleBoxType.Sprite)
            {
                if (image.sprite != style.backgroundSprite)
                {
                    image.sprite = style.backgroundSprite;
                    changed = true;
                }

                Color targetColor = style.tintColor;
                if (image.sprite == null)
                {
                    targetColor = new Color(0, 0, 0, 0);
                }

                if (image.color != targetColor)
                {
                    image.color = targetColor;
                    changed = true;
                }
            }
            else if (style.backgroundType == ModularStyleBox.StyleBoxType.None)
            {
                Color targetColor = new Color(0, 0, 0, 0);
                if (image.color != targetColor)
                {
                    image.color = targetColor;
                    changed = true;
                }
            }

            if (changed)
            {
                MarkAsDirty(image);
                MarkAsDirty(this);
            }
        }

        protected void MarkAsDirty(Object targetObj)
        {
            #if UNITY_EDITOR
                if (!Application.isPlaying && targetObj != null && shouldMarkDirty)
                {
                    UnityEditor.EditorUtility.SetDirty(targetObj);
                }
            #endif
        }
    }
}