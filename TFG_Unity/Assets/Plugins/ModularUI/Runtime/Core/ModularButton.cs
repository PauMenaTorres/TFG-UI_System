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

        [Header("Button Content")]
        [TextArea(3, 10)]
        [SerializeField] private string buttonText = "Modular Button";

        [SerializeField] private TextRole textFontRole = TextRole.Body;
        [SerializeField] private TextAlignmentOptions textAlignment = TextAlignmentOptions.Center;
        [SerializeField] private FontStyles fontStyle = FontStyles.Bold;
        [SerializeField] private Color textColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);

        [Header("Overrides")]
        [SerializeField] private AudioClip overrideClickSound;
        [SerializeField] private ModularStyleBox overrideNormalBG = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);
        [SerializeField] private ModularStyleBox overrideHoveredBG = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);
        [SerializeField] private ModularStyleBox overridePressedBG = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);
        [SerializeField] private ModularStyleBox overrideDisabledBG = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);
        [SerializeField] private TMP_FontAsset overrideFont;
        [SerializeField] private float overrideFontSize = 24.0f;

        private Button targetButton;
        private Image buttonImage;
        private TextMeshProUGUI textComponent;

        protected override void OnEnable()
        {
            base.OnEnable();
            FetchReferences();

            if (targetButton != null)
            {
                targetButton.onClick.AddListener(PlayClickSound);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (targetButton != null)
            {
                targetButton.onClick.RemoveListener(PlayClickSound);
            }
        }

        public void UpdateButtonText(string newText)
        {
            if (buttonText != newText)
            {
                buttonText = newText;

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
            FetchReferences();

            if (currentTheme == null)
            {
                return;
            }

            ModularStyleBox activeNormal = useOverride ? overrideNormalBG : currentTheme.buttonNormal;
            ModularStyleBox activeHovered = useOverride ? overrideHoveredBG : currentTheme.buttonHovered;
            ModularStyleBox activePressed = useOverride ? overridePressedBG : currentTheme.buttonPressed;
            ModularStyleBox activeDisabled = useOverride ? overrideDisabledBG : currentTheme.buttonDisabled;

            bool bgChanged = ApplyBackgroundTransitions(activeNormal, activeHovered, activePressed, activeDisabled);
            bool textChanged = ApplyTextStyles();

            if (bgChanged || textChanged)
            {
                MarkAsDirty(this);
            }
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

        private bool ApplyTextStyles()
        {
            if (textComponent == null)
            {
                return false;
            }

            bool changed = false;

            if (textComponent.text != buttonText)
            {
                textComponent.text = buttonText;
                changed = true;
            }

            if (textComponent.alignment != textAlignment)
            {
                textComponent.alignment = textAlignment;
                changed = true;
            }

            if (textComponent.fontStyle != fontStyle)
            {
                textComponent.fontStyle = fontStyle;
                changed = true;
            }

            if (useOverride)
            {
                if (textComponent.color != textColor)
                {
                    textComponent.color = textColor;
                    changed = true;
                }

                if (overrideFont != null)
                {
                    if (textComponent.font != overrideFont)
                    {
                        textComponent.font = overrideFont;
                        changed = true;
                    }
                }

                if (textComponent.fontSize != overrideFontSize)
                {
                    textComponent.fontSize = overrideFontSize;
                    changed = true;
                }
            }
            else
            {
                TMP_FontAsset targetFont = currentTheme.GetTextFont();
                float targetSize = currentTheme.TextFontSize;
                Color targetColor = currentTheme.textColor;

                if (textFontRole == TextRole.Title)
                {
                    targetFont = currentTheme.GetTitleFont();
                    targetSize = currentTheme.titleFontSize;
                    targetColor = currentTheme.titleColor;
                }

                if (textComponent.font != targetFont)
                {
                    textComponent.font = targetFont;
                    changed = true;
                }

                if (textComponent.fontSize != targetSize)
                {
                    textComponent.fontSize = targetSize;
                    changed = true;
                }

                if (textComponent.color != targetColor)
                {
                    textComponent.color = targetColor;
                    changed = true;
                }
            }

            if (changed)
            {
                MarkAsDirty(textComponent);
            }

            return changed;
        }

        private bool ApplyBackgroundTransitions(ModularStyleBox normal, ModularStyleBox hovered, ModularStyleBox pressed, ModularStyleBox disabled)
        {
            if (buttonImage == null || targetButton == null)
            {
                return false;
            }

            bool changed = false;

            if (normal.backgroundType == ModularStyleBox.StyleBoxType.SolidColor)
            {
                if (buttonImage.sprite != null)
                {
                    buttonImage.sprite = null;
                    changed = true;
                }

                if (buttonImage.color != normal.backgroundColor)
                {
                    buttonImage.color = normal.backgroundColor;
                    changed = true;
                }

                if (targetButton.transition != Selectable.Transition.ColorTint)
                {
                    targetButton.transition = Selectable.Transition.ColorTint;
                    changed = true;
                }

                ColorBlock colorBlock = targetButton.colors;
                bool colorsChanged = false;

                if (colorBlock.normalColor != normal.backgroundColor || 
                    colorBlock.highlightedColor != hovered.backgroundColor || 
                    colorBlock.pressedColor != pressed.backgroundColor || 
                    colorBlock.disabledColor != disabled.backgroundColor || 
                    colorBlock.selectedColor != normal.backgroundColor)
                {
                    colorsChanged = true;
                }

                if (colorsChanged)
                {
                    colorBlock.normalColor = normal.backgroundColor;
                    colorBlock.highlightedColor = hovered.backgroundColor;
                    colorBlock.pressedColor = pressed.backgroundColor;
                    colorBlock.disabledColor = disabled.backgroundColor;
                    colorBlock.selectedColor = normal.backgroundColor;
                    targetButton.colors = colorBlock;
                    changed = true;
                }
            }

            if (normal.backgroundType == ModularStyleBox.StyleBoxType.Sprite)
            {
                if (buttonImage.sprite != normal.backgroundSprite)
                {
                    buttonImage.sprite = normal.backgroundSprite;
                    changed = true;
                }

                if (buttonImage.color != new Color(1.0f, 1.0f, 1.0f, 1.0f))
                {
                    buttonImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    changed = true;
                }

                if (targetButton.transition != Selectable.Transition.SpriteSwap)
                {
                    targetButton.transition = Selectable.Transition.SpriteSwap;
                    changed = true;
                }

                SpriteState spriteState = targetButton.spriteState;
                bool spritesChanged = false;

                if (spriteState.highlightedSprite != hovered.backgroundSprite || 
                    spriteState.pressedSprite != pressed.backgroundSprite || 
                    spriteState.disabledSprite != disabled.backgroundSprite || 
                    spriteState.selectedSprite != normal.backgroundSprite)
                {
                    spritesChanged = true;
                }

                if (spritesChanged)
                {
                    spriteState.highlightedSprite = hovered.backgroundSprite;
                    spriteState.pressedSprite = pressed.backgroundSprite;
                    spriteState.disabledSprite = disabled.backgroundSprite;
                    spriteState.selectedSprite = normal.backgroundSprite;
                    targetButton.spriteState = spriteState;
                    changed = true;
                }
            }

            if (normal.backgroundType == ModularStyleBox.StyleBoxType.None)
            {
                if (targetButton.transition != Selectable.Transition.None)
                {
                    targetButton.transition = Selectable.Transition.None;
                    changed = true;
                }

                if (buttonImage.color != new Color(0.0f, 0.0f, 0.0f, 0.0f))
                {
                    buttonImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                    changed = true;
                }
            }

            if (changed)
            {
                MarkAsDirty(buttonImage);
                MarkAsDirty(targetButton);
            }

            return changed;
        }

        private void PlayClickSound()
        {
            AudioClip clip = null;

            if (useOverride && overrideClickSound != null)
            {
                clip = overrideClickSound;
            }

            if (!useOverride && currentTheme != null)
            {
                clip = currentTheme.defaultClickSound;
            }

            if (clip != null)
            {
                ModularAudioManager.Instance.PlaySound(clip);
            }
        }
    }
}