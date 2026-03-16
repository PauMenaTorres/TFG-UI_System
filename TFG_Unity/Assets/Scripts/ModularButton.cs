using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ModularButton : ModularComponents
{
    private Button targetButton;
    protected override void Awake()
    {
        targetButton = GetComponent<Button>();

        base.Awake();
    }

    public override void ApplyTheme()
    {
        base.ApplyTheme();

        if (targetButton == null)
        {
            targetButton = GetComponent<Button>();
        }

        // 2. EL ESCUDO: Si no hay tema, no hay botón, o activaste el override manual, abortamos y no hay error.
        if (currentTheme == null || targetButton == null || useOverride) return;

        ColorBlock buttonColors = targetButton.colors;

        buttonColors.normalColor = currentTheme.primaryColor;
        buttonColors.highlightedColor = currentTheme.secondaryColor;
        buttonColors.pressedColor = currentTheme.primaryColor;
        buttonColors.selectedColor = currentTheme.primaryColor;

        targetButton.colors = buttonColors;


    }
}
