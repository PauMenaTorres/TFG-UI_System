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

        [SerializeField] private Font overrideFont;
        [SerializeField] private float overrideFontSize = 24.0f;

        private TextMeshProUGUI textComponent;
        private bool lastOverrideState;

        protected override void Awake()
        {
            textComponent = GetComponent<TextMeshProUGUI>();
            base.Awake();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (textComponent == null)
            {
                textComponent = GetComponent<TextMeshProUGUI>();
            }

            if (useOverride && lastOverrideState == false)
            {
                if (currentTheme != null)
                {
                    if (textRole == TextRole.Title)
                    {
                        overrideFont = currentTheme.titleFont;
                        overrideFontSize = currentTheme.titleFontSize;
                    }
                    else
                    {
                        overrideFont = currentTheme.textFont;
                        overrideFontSize = currentTheme.textFontSize;
                    }
                }
            }

            bool isButtonChild = transform.parent != null && transform.parent.GetComponent<UnityEngine.UI.Button>() != null;
            if (!isButtonChild && textComponent != null)
            {
                textComponent.text = textContent;
            }

            lastOverrideState = useOverride;
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

            textComponent.alignment = alignment;
            textComponent.fontStyle = fontStyle;

            if (useOverride)
            {
                if (overrideFont != null)
                {
                    textComponent.font = TMP_FontAsset.CreateFontAsset(overrideFont);
                }
                textComponent.fontSize = overrideFontSize;
            }
            else
            {
                if (textRole == TextRole.Title)
                {
                    textComponent.font = currentTheme.GetTitleFont();
                    textComponent.fontSize = currentTheme.titleFontSize;
                }
                else
                {
                    textComponent.font = currentTheme.GetTextFont();
                    textComponent.fontSize = currentTheme.textFontSize;
                }
            }

            if (colorRole == TextColorRole.Primary)
            {
                textComponent.color = currentTheme.primaryColor;
            }
            else if (colorRole == TextColorRole.Secondary)
            {
                textComponent.color = currentTheme.secondaryColor;
            }
            else
            {
                textComponent.color = customColor;
            }

            textComponent.SetAllDirty();
        }
    }
}