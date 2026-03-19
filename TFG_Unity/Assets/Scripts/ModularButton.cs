using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    [RequireComponent(typeof(Button))]
    public class ModularButton : ModularComponents
    {
        public ModularStyleBox overrideNormal;
        public ModularStyleBox overrideHovered;
        public ModularStyleBox overridePressed;
        public ModularStyleBox overrideDisabled;

        private Button targetButton;
        private Image buttonImage;
        private bool lastOverrideState;
        public string buttonText = "Button Text";
        private TextMeshProUGUI buttonTextChild;

        protected override void Awake()
        {
            targetButton = GetComponent<Button>();
            buttonImage = GetComponent<Image>();

            base.Awake();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (useOverride && lastOverrideState == false)
            {
                if (currentTheme != null)
                {
                    overrideNormal = currentTheme.buttonNormal;
                    overrideHovered = currentTheme.buttonHovered;
                    overridePressed = currentTheme.buttonPressed;
                    overrideDisabled = currentTheme.buttonDisabled;
                }
            }

            if (buttonTextChild == null)
            {
                buttonTextChild = GetComponentInChildren<TextMeshProUGUI>();
            }

            if (buttonTextChild != null && buttonTextChild.text != buttonText)
            {
                buttonTextChild.text = buttonText;
            }

            lastOverrideState = useOverride;
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (targetButton == null)
            {
                targetButton = GetComponent<Button>();
            }

            if (buttonImage == null)
            {
                buttonImage = GetComponent<Image>();
            }

            if (currentTheme == null)
            {
                return;
            }

            ModularStyleBox activeNormal;
            ModularStyleBox activeHovered;
            ModularStyleBox activePressed;
            ModularStyleBox activeDisabled;

            if (useOverride)
            {
                activeNormal = overrideNormal;
                activeHovered = overrideHovered;
                activePressed = overridePressed;
                activeDisabled = overrideDisabled;
            }
            else
            {
                activeNormal = currentTheme.buttonNormal;
                activeHovered = currentTheme.buttonHovered;
                activePressed = currentTheme.buttonPressed;
                activeDisabled = currentTheme.buttonDisabled;
            }


            if (activeNormal.backgroundType == ModularStyleBox.StyleBoxType.SolidColor)
            {
                buttonImage.sprite = null;
                buttonImage.color = activeNormal.backgroundColor;

                targetButton.transition = Selectable.Transition.ColorTint;

                ColorBlock colorBlock = targetButton.colors;
                colorBlock.normalColor = activeNormal.backgroundColor;
                colorBlock.highlightedColor = activeHovered.backgroundColor;
                colorBlock.pressedColor = activePressed.backgroundColor;
                colorBlock.disabledColor = activeDisabled.backgroundColor;
                colorBlock.selectedColor = activeNormal.backgroundColor;

                targetButton.colors = colorBlock;
            }
            else if (activeNormal.backgroundType == ModularStyleBox.StyleBoxType.Sprite)
            {
                buttonImage.sprite = activeNormal.backgroundSprite;
                buttonImage.color = Color.white;

                targetButton.transition = Selectable.Transition.SpriteSwap;

                SpriteState spriteState = targetButton.spriteState;
                spriteState.highlightedSprite = activeHovered.backgroundSprite;
                spriteState.pressedSprite = activePressed.backgroundSprite;
                spriteState.disabledSprite = activeDisabled.backgroundSprite;
                spriteState.selectedSprite = activeNormal.backgroundSprite;

                targetButton.spriteState = spriteState;
            }
            else
            {
                targetButton.transition = Selectable.Transition.None;
                buttonImage.color = Color.clear;
            }
        }
    }
}