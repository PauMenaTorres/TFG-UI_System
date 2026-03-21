using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ModularUIRuntime
{
    [RequireComponent(typeof(Button))]
    public class ModularButton : ModularComponents
    {
        public enum TextRole
        {
            Body,
            Title
        }

        [Header("Button Content (Proxied to Text Child)")]
        [SerializeField] private string buttonText = "Modular Button";
        [SerializeField] private TextRole textFontRole = TextRole.Body;
        [SerializeField] private TextAlignmentOptions textAlignment = TextAlignmentOptions.Center;
        [SerializeField] private FontStyles fontStyle = FontStyles.Bold;
        [SerializeField] private Color textColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);

        [Header("Background Styles")]
        [SerializeField] private ModularStyleBox backgroundNormal = new ModularStyleBox { backgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) };
        [SerializeField] private ModularStyleBox backgroundHovered = new ModularStyleBox { backgroundColor = new Color(0.9f, 0.9f, 0.9f, 1.0f) };
        [SerializeField] private ModularStyleBox backgroundPressed = new ModularStyleBox { backgroundColor = new Color(0.7f, 0.7f, 0.7f, 1.0f) };
        [SerializeField] private ModularStyleBox backgroundDisabled = new ModularStyleBox { backgroundColor = new Color(0.4f, 0.4f, 0.4f, 1.0f) };

        [Header("Overrides")]
        [SerializeField] private ModularStyleBox overrideNormalBG = new ModularStyleBox { backgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) };
        [SerializeField] private ModularStyleBox overrideHoveredBG = new ModularStyleBox { backgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) };
        [SerializeField] private ModularStyleBox overridePressedBG = new ModularStyleBox { backgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) };
        [SerializeField] private ModularStyleBox overrideDisabledBG = new ModularStyleBox { backgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) };
        [SerializeField] private Font overrideFont;
        [SerializeField] private float overrideFontSize = 24.0f;

        private Button targetButton;
        private Image buttonImage;
        private TextMeshProUGUI textComponent;
        private bool lastOverrideState;

        protected override void OnValidate()
        {
            FetchReferences();

            if (textComponent != null)
            {
                textComponent.text = buttonText;
                textComponent.alignment = textAlignment;
                textComponent.fontStyle = fontStyle;
                textComponent.color = textColor;
                textComponent.SetAllDirty();
            }

            base.OnValidate();

            if (useOverride && lastOverrideState == false)
            {
                if (currentTheme != null)
                {
                    overrideNormalBG = backgroundNormal;
                    overrideHoveredBG = backgroundHovered;
                    overridePressedBG = backgroundPressed;
                    overrideDisabledBG = backgroundDisabled;
                    overrideFont = textFontRole == TextRole.Title ? currentTheme.titleFont : currentTheme.textFont;
                    overrideFontSize = textFontRole == TextRole.Title ? currentTheme.titleFontSize : currentTheme.textFontSize;
                }
            }
            lastOverrideState = useOverride;
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();
            FetchReferences();

            if (currentTheme == null) return;

            ModularStyleBox activeNormal = useOverride ? overrideNormalBG : backgroundNormal;
            ModularStyleBox activeHovered = useOverride ? overrideHoveredBG : backgroundHovered;
            ModularStyleBox activePressed = useOverride ? overridePressedBG : backgroundPressed;
            ModularStyleBox activeDisabled = useOverride ? overrideDisabledBG : backgroundDisabled;

            ApplyBackgroundTransitions(activeNormal, activeHovered, activePressed, activeDisabled);
            ApplyTextStyles();
        }

        private void FetchReferences()
        {
            if (targetButton == null)
            {
                targetButton = GetComponent<Button>();
            }

            if (buttonImage == null)
            {
                buttonImage = GetComponent<Image>();
            }

            if (textComponent == null)
            {
                textComponent = GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        private void ApplyTextStyles()
        {
            if (textComponent == null) return;

            textComponent.text = buttonText;
            textComponent.alignment = textAlignment;
            textComponent.fontStyle = fontStyle;
            textComponent.color = textColor;

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
                if (textFontRole == TextRole.Title)
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
            textComponent.SetAllDirty();
        }

        private void ApplyBackgroundTransitions(ModularStyleBox normal, ModularStyleBox hovered, ModularStyleBox pressed, ModularStyleBox disabled)
        {
            if (buttonImage == null || targetButton == null) return;

            if (normal.backgroundType == ModularStyleBox.StyleBoxType.SolidColor)
            {
                buttonImage.sprite = null;
                buttonImage.color = normal.backgroundColor;

                targetButton.transition = Selectable.Transition.ColorTint;

                ColorBlock colorBlock = targetButton.colors;
                colorBlock.normalColor = normal.backgroundColor;
                colorBlock.highlightedColor = hovered.backgroundColor;
                colorBlock.pressedColor = pressed.backgroundColor;
                colorBlock.disabledColor = disabled.backgroundColor;
                colorBlock.selectedColor = normal.backgroundColor;
                targetButton.colors = colorBlock;
            }
            else if (normal.backgroundType == ModularStyleBox.StyleBoxType.Sprite)
            {
                buttonImage.sprite = normal.backgroundSprite;
                buttonImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

                targetButton.transition = Selectable.Transition.SpriteSwap;

                SpriteState spriteState = targetButton.spriteState;
                spriteState.highlightedSprite = hovered.backgroundSprite;
                spriteState.pressedSprite = pressed.backgroundSprite;
                spriteState.disabledSprite = disabled.backgroundSprite;
                spriteState.selectedSprite = normal.backgroundSprite;
                targetButton.spriteState = spriteState;
            }
            else
            {
                targetButton.transition = Selectable.Transition.None;
                buttonImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            }
        }
    }
}