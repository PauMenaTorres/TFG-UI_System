using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ModularButton : ModularComponents
{
    private Button targetButton;
    private Image buttonImage;

    protected override void Awake()
    {
        targetButton = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        base.Awake();
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

        if (currentTheme == null || useOverride)
        {
            return;
        }

        ModularStyleBox normalStyle = currentTheme.buttonNormal;

        if (normalStyle.backgroundType == ModularStyleBox.StyleBoxType.SolidColor)
        {
            buttonImage.sprite = null;
            buttonImage.color = normalStyle.backgroundColor;

            targetButton.transition = Selectable.Transition.ColorTint;

            ColorBlock colorBlock = targetButton.colors;
            colorBlock.normalColor = normalStyle.backgroundColor;
            colorBlock.highlightedColor = currentTheme.buttonHovered.backgroundColor;
            colorBlock.pressedColor = currentTheme.buttonPressed.backgroundColor;
            colorBlock.disabledColor = currentTheme.buttonDisabled.backgroundColor;
            colorBlock.selectedColor = normalStyle.backgroundColor;

            targetButton.colors = colorBlock;
        }
        else if (normalStyle.backgroundType == ModularStyleBox.StyleBoxType.Sprite)
        {
            buttonImage.sprite = normalStyle.backgroundSprite;
            buttonImage.color = Color.white;

            targetButton.transition = Selectable.Transition.SpriteSwap;

            SpriteState spriteState = targetButton.spriteState;
            spriteState.highlightedSprite = currentTheme.buttonHovered.backgroundSprite;
            spriteState.pressedSprite = currentTheme.buttonPressed.backgroundSprite;
            spriteState.disabledSprite = currentTheme.buttonDisabled.backgroundSprite;
            spriteState.selectedSprite = normalStyle.backgroundSprite;

            targetButton.spriteState = spriteState;
        }
        else
        {
            targetButton.transition = Selectable.Transition.None;
            buttonImage.color = Color.clear;
        }
    }
}