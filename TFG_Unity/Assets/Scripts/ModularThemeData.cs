using UnityEngine;
using System;
using TMPro;

[Serializable]
public struct ModularStyleBox
{
    public enum StyleBoxType 
    { 
        SolidColor,
        Sprite,
        None
    }

    [Tooltip("Choose between solid color or sprite.")]
    public StyleBoxType backgroundType;

    public Color backgroundColor;
    public Sprite backgroundSprite;
}

[CreateAssetMenu(fileName = "ModularThemeData", menuName = "Scriptable Objects/ModularThemeData")]
public class ModularThemeData : ScriptableObject
{
    public event Action OnThemeChanged;

    [Header("Global Colors")]
    public Color primaryColor = Color.white;
    public Color secondaryColor = Color.black;

    [Header("Typography")]
    public Font textFont;
    public Font titleFont;
    public float textFontSize = 24.0f;
    public float titleFontSize = 36.0f;

    private TMP_FontAsset generatedTMPTextFont;
    private TMP_FontAsset generatedTMPTitleFont;

    [Header("Button States (StyleBoxes)")]
    public ModularStyleBox buttonNormal = new ModularStyleBox { backgroundColor = Color.white };
    public ModularStyleBox buttonHovered = new ModularStyleBox { backgroundColor = Color.gray };
    public ModularStyleBox buttonPressed = new ModularStyleBox { backgroundColor = Color.darkGray };
    public ModularStyleBox buttonDisabled = new ModularStyleBox { backgroundColor = Color.clear };
    public TMP_FontAsset GetTextFont()
    {
        if (generatedTMPTextFont == null && textFont != null)
        {
            generatedTMPTextFont = TMP_FontAsset.CreateFontAsset(textFont);
        }
        return generatedTMPTextFont;
    }

    public TMP_FontAsset GetTitleFont()
    {
        if (generatedTMPTitleFont == null && titleFont != null)
        {
            generatedTMPTitleFont = TMP_FontAsset.CreateFontAsset(titleFont);
        }
        return generatedTMPTitleFont;
    }
    private void OnValidate()
    {
        generatedTMPTextFont = null;
        generatedTMPTitleFont = null;

        OnThemeChanged?.Invoke();
    }
}