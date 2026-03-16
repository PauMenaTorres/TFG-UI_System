using UnityEngine;
using TMPro;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "ModularThemeData", menuName = "Scriptable Objects/ModularThemeData")]
public class ModularThemeData : ScriptableObject
{
    [Header("Colors")]
    public Color primaryColor = Color.white;
    public Color secondaryColor = Color.black;

    [Header("Typhography")]
    public Font textFont;
    public Font titleFont;
    public float baseFontSize = 24.0f;
    public float headerFontSize = 36.0f;

    private TMP_FontAsset generatedTMPTextFont;
    private TMP_FontAsset generatedTMPTitleFont;

    [Header("Button States Background")]
    public Sprite buttonNormalBackground;
    public Sprite buttonPressedBackground;
    public Sprite buttonHoveredBackground;
    public Sprite buttonDisabledBackground;



    public TMP_FontAsset GetTextFont()
    {
        if (generatedTMPTextFont == null)
        {
            generatedTMPTextFont = TMP_FontAsset.CreateFontAsset(textFont);
        }

        return generatedTMPTextFont;
    }
    public TMP_FontAsset GetTitleFont()
    {
        if (generatedTMPTitleFont == null)
        {
            generatedTMPTitleFont = TMP_FontAsset.CreateFontAsset(titleFont);
        }

        return generatedTMPTitleFont;
    }
}
