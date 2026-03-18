using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ModularText : ModularComponents
{
    public enum TextStyle
    {
        Body,
        Title
    }

    public TextStyle textRole;

    public Color overridePrimaryColor;
    public Color overrideSecondaryColor;
    public Font overrideTextFont;
    public Font overrideTitleFont;
    public float overrideTextFontSize = 24.0f;
    public float overrideTitleFontSize = 36.0f;

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

        if (useOverride && lastOverrideState == false)
        {
            if (currentTheme != null)
            {
                overridePrimaryColor = currentTheme.primaryColor;
                overrideSecondaryColor = currentTheme.secondaryColor;
                overrideTextFont = currentTheme.textFont;
                overrideTitleFont = currentTheme.titleFont;
                overrideTextFontSize = currentTheme.textFontSize;
                overrideTitleFontSize = currentTheme.titleFontSize;
            }
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

        Font activeTextFont;
        Font activeTitleFont;
        Color activePrimaryColor;
        Color activeSecondaryColor;
        float activeTextFontSize;
        float activeTitleFontSize;

        if (useOverride)
        {
            activeTextFont = overrideTextFont;
            activeTitleFont = overrideTitleFont;
            activePrimaryColor = overridePrimaryColor;
            activeSecondaryColor = overrideSecondaryColor;
            activeTextFontSize = overrideTextFontSize;
            activeTitleFontSize = overrideTitleFontSize;
        }
        else
        {
            activeTextFont = currentTheme.textFont;
            activeTitleFont = currentTheme.titleFont;
            activePrimaryColor = currentTheme.primaryColor;
            activeSecondaryColor = currentTheme.secondaryColor;
            activeTextFontSize = currentTheme.textFontSize;
            activeTitleFontSize = currentTheme.titleFontSize;
        }

        if (textRole == TextStyle.Title)
        {
            if (useOverride && activeTitleFont != null)
            {
                textComponent.font = TMP_FontAsset.CreateFontAsset(activeTitleFont);
            }
            else
            {
                textComponent.font = currentTheme.GetTitleFont();
            }

            textComponent.fontSize = activeTitleFontSize;
            textComponent.color = activePrimaryColor;
        }
        else if (textRole == TextStyle.Body)
        {
            if (useOverride && activeTextFont != null)
            {
                textComponent.font = TMP_FontAsset.CreateFontAsset(activeTextFont);
            }
            else
            {
                textComponent.font = currentTheme.GetTextFont();
            }

            textComponent.fontSize = activeTextFontSize;
            textComponent.color = activeSecondaryColor;
        }

        textComponent.SetAllDirty();
    }
}