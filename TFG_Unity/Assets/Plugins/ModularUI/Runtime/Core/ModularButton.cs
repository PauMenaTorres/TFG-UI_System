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
        [SerializeField] private Font overrideFont;
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
            if (textComponent == null)
            {
                return;
            }

            if (textComponent.text != buttonText)
            {
                textComponent.text = buttonText;
            }

            if (textComponent.alignment != textAlignment)
            {
                textComponent.alignment = textAlignment;
            }

            if (textComponent.fontStyle != fontStyle)
            {
                textComponent.fontStyle = fontStyle;
            }

            if (textComponent.color != textColor)
            {
                textComponent.color = textColor;
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

                if (textComponent.fontSize != overrideFontSize)
                {
                    textComponent.fontSize = overrideFontSize;
                }
            }

            if (!useOverride)
            {
                TMP_FontAsset targetFont = currentTheme.GetTextFont();
                float targetSize = currentTheme.textFontSize;

                if (textFontRole == TextRole.Title)
                {
                    targetFont = currentTheme.GetTitleFont();
                    targetSize = currentTheme.titleFontSize;
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
        }

        private void ApplyBackgroundTransitions(ModularStyleBox normal, ModularStyleBox hovered, ModularStyleBox pressed, ModularStyleBox disabled)
        {
            if (buttonImage == null || targetButton == null)
            {
                return;
            }

            if (normal.backgroundType == ModularStyleBox.StyleBoxType.SolidColor)
            {
                if (buttonImage.sprite != null)
                {
                    buttonImage.sprite = null;
                }

                if (buttonImage.color != normal.backgroundColor)
                {
                    buttonImage.color = normal.backgroundColor;
                }

                if (targetButton.transition != Selectable.Transition.ColorTint)
                {
                    targetButton.transition = Selectable.Transition.ColorTint;
                }

                ColorBlock colorBlock = targetButton.colors;
                bool colorsChanged = false;

                if (colorBlock.normalColor != normal.backgroundColor || colorBlock.highlightedColor != hovered.backgroundColor || colorBlock.pressedColor != pressed.backgroundColor || colorBlock.disabledColor != disabled.backgroundColor || colorBlock.selectedColor != normal.backgroundColor)
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
                }
            }

            if (normal.backgroundType == ModularStyleBox.StyleBoxType.Sprite)
            {
                if (buttonImage.sprite != normal.backgroundSprite)
                {
                    buttonImage.sprite = normal.backgroundSprite;
                }

                if (buttonImage.color != new Color(1.0f, 1.0f, 1.0f, 1.0f))
                {
                    buttonImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }

                if (targetButton.transition != Selectable.Transition.SpriteSwap)
                {
                    targetButton.transition = Selectable.Transition.SpriteSwap;
                }

                SpriteState spriteState = targetButton.spriteState;
                bool spritesChanged = false;

                if (spriteState.highlightedSprite != hovered.backgroundSprite || spriteState.pressedSprite != pressed.backgroundSprite || spriteState.disabledSprite != disabled.backgroundSprite || spriteState.selectedSprite != normal.backgroundSprite)
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
                }
            }

            if (normal.backgroundType == ModularStyleBox.StyleBoxType.None)
            {
                if (targetButton.transition != Selectable.Transition.None)
                {
                    targetButton.transition = Selectable.Transition.None;
                }

                if (buttonImage.color != new Color(0.0f, 0.0f, 0.0f, 0.0f))
                {
                    buttonImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                }
            }
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
                GameObject audioObj = new GameObject("UI_Click_Audio");
                AudioSource source = audioObj.AddComponent<AudioSource>();
                source.clip = clip;
                source.spatialBlend = 0f;
                source.Play();
                Destroy(audioObj, clip.length);
            }
        }
    }
}