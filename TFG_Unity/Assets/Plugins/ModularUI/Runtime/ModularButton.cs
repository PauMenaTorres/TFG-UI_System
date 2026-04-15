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
        private bool lastOverrideState;

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

        protected override void OnValidate()
        {
            if (useOverride && lastOverrideState == false)
            {
                if (currentTheme != null)
                {
                    overrideNormalBG = currentTheme.buttonNormal;
                    overrideHoveredBG = currentTheme.buttonHovered;
                    overridePressedBG = currentTheme.buttonPressed;
                    overrideDisabledBG = currentTheme.buttonDisabled;

                    if (textFontRole == TextRole.Title)
                    {
                        overrideFont = currentTheme.titleFont;
                        overrideFontSize = currentTheme.titleFontSize;
                    }

                    if (textFontRole == TextRole.Body)
                    {
                        overrideFont = currentTheme.textFont;
                        overrideFontSize = currentTheme.textFontSize;
                    }
                }
            }

            lastOverrideState = useOverride;
            base.OnValidate();
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

            if (!useOverride)
            {
                if (textFontRole == TextRole.Title)
                {
                    textComponent.font = currentTheme.GetTitleFont();
                    textComponent.fontSize = currentTheme.titleFontSize;
                }

                if (textFontRole == TextRole.Body)
                {
                    textComponent.font = currentTheme.GetTextFont();
                    textComponent.fontSize = currentTheme.textFontSize;
                }
            }

            textComponent.SetAllDirty();
        }

        private void ApplyBackgroundTransitions(ModularStyleBox normal, ModularStyleBox hovered, ModularStyleBox pressed, ModularStyleBox disabled)
        {
            if (buttonImage == null || targetButton == null)
            {
                return;
            }

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

            if (normal.backgroundType == ModularStyleBox.StyleBoxType.Sprite)
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

            if (normal.backgroundType == ModularStyleBox.StyleBoxType.None)
            {
                targetButton.transition = Selectable.Transition.None;
                buttonImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
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