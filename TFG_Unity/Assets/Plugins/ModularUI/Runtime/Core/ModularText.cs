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
            Secondary
        }

        [Header("Text Settings")]
        [SerializeField] private TextRole textRole = TextRole.Body;
        [SerializeField] private TextColorRole colorRole = TextColorRole.Secondary;
        [SerializeField] private Color customColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        [SerializeField] private string textContent = "Text Content";
        [SerializeField] private TextAlignmentOptions alignment = TextAlignmentOptions.Center;
        [SerializeField] private FontStyles fontStyle = FontStyles.Normal;

        [Header("Size Override")]
        [SerializeField] private bool useCustomFontSize = false;
        [SerializeField] private float customFontSize = 24.0f;

        [Header("Full Override Settings")]
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

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (textComponent == null)
            {
                textComponent = GetComponent<TextMeshProUGUI>();
            }

            if (currentTheme == null)
            {
                return;
            }

            if (textComponent.alignment != alignment)
            {
                textComponent.alignment = alignment;
            }

            if (textComponent.fontStyle != fontStyle)
            {
                textComponent.fontStyle = fontStyle;
            }

            if (useOverride)
            {
                if (overrideFont != null)
                {
                    if (textComponent.font == null || textComponent.font.sourceFontFile != overrideFont)
                    {
                        textComponent.font = TMP_FontAsset.CreateFontAsset(overrideFont);
                    }
                }

                float targetSize = useCustomFontSize ? customFontSize : overrideFontSize;

                if (textComponent.fontSize != targetSize)
                {
                    textComponent.fontSize = targetSize;
                }
            }

            if (!useOverride)
            {
                TMP_FontAsset targetFont = currentTheme.GetTextFont();
                float targetSize = useCustomFontSize ? customFontSize : currentTheme.textFontSize;

                if (textRole == TextRole.Title)
                {
                    targetFont = currentTheme.GetTitleFont();
                    targetSize = useCustomFontSize ? customFontSize : currentTheme.titleFontSize;
                }

                if (textComponent.font != targetFont)
                {
                    textComponent.font = targetFont;
                }

                if (textComponent.fontSize != targetSize)
                {
                    textComponent.fontSize = targetSize;
                }
            }

            Color targetColor = customColor;

            if (colorRole == TextColorRole.Primary)
            {
                targetColor = currentTheme.primaryColor;
            }

            if (colorRole == TextColorRole.Secondary)
            {
                targetColor = currentTheme.secondaryColor;
            }

            if (textComponent.color != targetColor)
            {
                textComponent.color = targetColor;
            }

            textComponent.SetAllDirty();
        }
    }
}