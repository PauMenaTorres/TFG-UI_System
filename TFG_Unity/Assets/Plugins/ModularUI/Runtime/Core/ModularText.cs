using UnityEngine;
using TMPro;

namespace ModularUIRuntime
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ModularText : ModularComponents
    {
        public enum TextRole
        {
            Body,
            Title
        }

        public enum TextColorRole
        {
            Custom,
            Primary,
            Secondary,
            ThemeDefault
        }

        [Header("Text Settings")]
        [SerializeField] private TextRole textRole = TextRole.Body;
        [SerializeField] private TextColorRole colorRole = TextColorRole.ThemeDefault;
        [SerializeField] private Color customColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        [TextArea(3, 10)]
        [SerializeField] private string textContent = "Text Content";

        [SerializeField] private TextAlignmentOptions alignment = TextAlignmentOptions.Center;
        [SerializeField] private FontStyles fontStyle = FontStyles.Normal;

        [Header("Size Override")]
        [SerializeField] private bool useCustomFontSize = false;
        [SerializeField] private float customFontSize = 24.0f;

        [Header("Full Override Settings")]
        [SerializeField] private TMP_FontAsset overrideFontAsset;
        [System.Obsolete("Use overrideFontAsset instead of overrideFont to avoid asset dirtying.")]
        [SerializeField] private Font overrideFont;
        [SerializeField] private float overrideFontSize = 24.0f;

        private TextMeshProUGUI textComponent;

        protected override void Awake()
        {
            textComponent = GetComponent<TextMeshProUGUI>();
            base.Awake();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return;
            }

            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null)
                {
                    return;
                }

                if (textComponent == null)
                {
                    textComponent = GetComponent<TextMeshProUGUI>();
                }

                bool isButtonChild = false;

                if (transform.parent != null)
                {
                    if (transform.parent.GetComponent<UnityEngine.UI.Button>() != null)
                    {
                        isButtonChild = true;
                    }
                }

                if (!isButtonChild && textComponent != null)
                {
                    if (textComponent.text != textContent)
                    {
                        textComponent.text = textContent;
                    }

                    ModularMainMenu parentMenu = GetComponentInParent<ModularMainMenu>();

                    if (parentMenu != null)
                    {
                        parentMenu.UpdateTextFromChild(this, textContent);
                    }
                }
            };
#endif
        }

        public void UpdateTextFromExternal(string newText)
        {
            if (textContent != newText)
            {
                textContent = newText;

                if (textComponent != null)
                {
                    if (textComponent.text != newText)
                    {
                        textComponent.text = newText;
                        textComponent.SetAllDirty();
                    }
                }
            }
        }

        private bool Approximately(float a, float b)
        {
            return Mathf.Abs(a - b) < 0.005f;
        }

        private bool Approximately(Color a, Color b)
        {
            return Mathf.Abs(a.r - b.r) < 0.005f &&
                   Mathf.Abs(a.g - b.g) < 0.005f &&
                   Mathf.Abs(a.b - b.b) < 0.005f &&
                   Mathf.Abs(a.a - b.a) < 0.005f;
        }

        public override void ApplyTheme()
        {
#if UNITY_EDITOR
            if (!gameObject.scene.IsValid())
            {
                var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject);
                if (stage == null)
                {
                    return;
                }
            }
#endif

            base.ApplyTheme();

            if (textComponent == null)
            {
                textComponent = GetComponent<TextMeshProUGUI>();
            }

            if (textComponent == null || currentTheme == null)
            {
                return;
            }

            bool changed = false;

            if (textComponent.alignment != alignment)
            {
                textComponent.alignment = alignment;
                changed = true;
            }

            if (textComponent.fontStyle != fontStyle)
            {
                textComponent.fontStyle = fontStyle;
                changed = true;
            }

            if (useOverride)
            {
                if (overrideFontAsset != null)
                {
                    if (textComponent.font != overrideFontAsset)
                    {
                        textComponent.font = overrideFontAsset;
                        changed = true;
                    }
                }
                else if (overrideFont != null)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                    }
                    else
#endif
                    {
                        if (textComponent.font == null || textComponent.font.sourceFontFile != overrideFont)
                        {
                            textComponent.font = TMP_FontAsset.CreateFontAsset(overrideFont);
                            changed = true;
                        }
                    }
                }

                float targetSize = useCustomFontSize ? customFontSize : overrideFontSize;

                if (!Approximately(textComponent.fontSize, targetSize))
                {
                    textComponent.fontSize = targetSize;
                    changed = true;
                }
            }
            else
            {
                TMP_FontAsset targetFont = currentTheme.GetTextFont();
                float targetSize = useCustomFontSize ? customFontSize : currentTheme.TextFontSize;

                if (textRole == TextRole.Title)
                {
                    targetFont = currentTheme.GetTitleFont();
                    targetSize = useCustomFontSize ? customFontSize : currentTheme.titleFontSize;
                }

                if (targetFont != null && textComponent.font != targetFont)
                {
                    textComponent.font = targetFont;
                    changed = true;
                }

                if (!Approximately(textComponent.fontSize, targetSize))
                {
                    textComponent.fontSize = targetSize;
                    changed = true;
                }
            }

            Color targetColor = (textRole == TextRole.Title) ? currentTheme.titleColor : currentTheme.textColor;

            if (colorRole == TextColorRole.Primary)
            {
                targetColor = currentTheme.primaryColor;
            }
            else if (colorRole == TextColorRole.Secondary)
            {
                targetColor = currentTheme.secondaryColor;
            }
            else if (colorRole == TextColorRole.Custom)
            {
                targetColor = customColor;
            }
            else if (colorRole == TextColorRole.ThemeDefault)
            {
                targetColor = (textRole == TextRole.Title) ? currentTheme.titleColor : currentTheme.textColor;
            }

            if (!Approximately(textComponent.color, targetColor))
            {
                textComponent.color = targetColor;
                changed = true;
            }

            if (changed)
            {
                textComponent.SetAllDirty();
                MarkAsDirty(textComponent);
                MarkAsDirty(this);
            }
        }
    }
}