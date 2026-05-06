using UnityEngine;
using System;
using TMPro;

namespace ModularUIRuntime
{
    [Serializable]
    public struct ModularStyleBox
    {
        public enum StyleBoxType
        {
            SolidColor,
            Sprite,
            None
        }

        public StyleBoxType backgroundType;
        public Color backgroundColor;
        public Sprite backgroundSprite;
        public Color tintColor;

        public ModularStyleBox(StyleBoxType type)
        {
            backgroundType = type;
            backgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            backgroundSprite = null;
            tintColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
    }

    [CreateAssetMenu(fileName = "ModularThemeData", menuName = "Modular UI/ModularThemeData")]
    public class ModularThemeData : ScriptableObject
    {
        public event Action OnThemeChanged;

        [Header("Global Settings")]
        public Color primaryColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public Color secondaryColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        public AudioClip defaultClickSound;

        [Header("Typography")]
        public Font textFont;
        public Font titleFont;
        public float textFontSize = 24.0f;
        public float titleFontSize = 36.0f;

        private TMP_FontAsset generatedTMPTextFont;
        private TMP_FontAsset generatedTMPTitleFont;

        [Header("Button States")]
        public ModularStyleBox buttonNormal = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);
        public ModularStyleBox buttonHovered = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);
        public ModularStyleBox buttonPressed = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);
        public ModularStyleBox buttonDisabled = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);

        [Header("Slider States")]
        public ModularStyleBox sliderBackground = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);
        public ModularStyleBox sliderFill = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);
        public ModularStyleBox sliderHandle = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);

        [Header("Toggle States")]
        public ModularStyleBox toggleBackground = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);
        public ModularStyleBox toggleCheckmark = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);

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
}