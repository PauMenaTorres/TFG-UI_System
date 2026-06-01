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

        protected bool canModifyComponents
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying && !shouldMarkDirty)
                    return false;
#endif
                return true;
            }
        }

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

            if (Application.isPlaying && ModularThemeManager.Instance != null)
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

            if (Application.isPlaying && ModularThemeManager.HasInstance && ModularThemeManager.Instance != null)
            {
                ModularThemeManager.Instance.OnThemeChanged -= HandleThemeChanged;
            }
        }

        private void HandleThemeChanged()
        {
            ApplyTheme();
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

        private bool Approximately(Color a, Color b)
        {
            return Mathf.Abs(a.r - b.r) < 0.005f &&
                   Mathf.Abs(a.g - b.g) < 0.005f &&
                   Mathf.Abs(a.b - b.b) < 0.005f &&
                   Mathf.Abs(a.a - b.a) < 0.005f;
        }

        public virtual void ApplyTheme()
        {
            #if UNITY_EDITOR
            if (UnityEditor.EditorUtility.IsPersistent(this))
            {
                return;
            }

            if (!gameObject.scene.IsValid())
            {
                var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject);
                if (stage == null)
                {
                    return;
                }
            }
            #endif

            if (!useOverride)
            {
                ModularThemeData activeTheme = null;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UIConfiguration config = Resources.Load<UIConfiguration>("UIConfiguration");
                    if (config == null)
                    {
                        config = UnityEditor.AssetDatabase.LoadAssetAtPath<UIConfiguration>("Assets/Plugins/ModularUI/Settings/UIConfiguration.asset");
                    }
                    if (config != null)
                    {
                        activeTheme = config.GetActiveTheme();
                    }
                }
                else
#endif
                {
                    ModularThemeManager manager = ModularThemeManager.Instance;
                    if (manager != null)
                    {
                        activeTheme = manager.GetActiveTheme();
                    }
                }

                if (activeTheme != null)
                {
                    if (currentTheme != activeTheme)
                    {
                        if (currentTheme != null && Application.isPlaying) currentTheme.OnThemeChanged -= HandleThemeChanged;
                        currentTheme = activeTheme;
                        if (currentTheme != null && Application.isPlaying) currentTheme.OnThemeChanged += HandleThemeChanged;
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

        public void ApplyThemeInEditor()
        {
#if UNITY_EDITOR
            bool oldMarkDirty = shouldMarkDirty;
            shouldMarkDirty = true;
            ApplyTheme();
            shouldMarkDirty = oldMarkDirty;
#endif
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
                if (!Approximately(image.color, style.backgroundColor))
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

                if (image.sprite != null && image.sprite.border != Vector4.zero)
                {
                    if (image.type != Image.Type.Sliced)
                    {
                        image.type = Image.Type.Sliced;
                        changed = true;
                    }
                }

                Color targetColor = style.tintColor;
                if (image.sprite == null)
                {
                    targetColor = new Color(0, 0, 0, 0);
                }

                if (!Approximately(image.color, targetColor))
                {
                    image.color = targetColor;
                    changed = true;
                }
            }
            else if (style.backgroundType == ModularStyleBox.StyleBoxType.None)
            {
                Color targetColor = new Color(0, 0, 0, 0);
                if (!Approximately(image.color, targetColor))
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