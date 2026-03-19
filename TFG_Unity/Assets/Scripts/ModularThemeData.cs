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

        [Tooltip("Choose between solid color or sprite.")]
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

    [CreateAssetMenu(fileName = "ModularThemeData", menuName = "Scriptable Objects/ModularThemeData")]
    public class ModularThemeData : ScriptableObject
    {
        public event Action OnThemeChanged;

        [Header("Global Colors")]
        public Color primaryColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        public Color secondaryColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);

        [Header("Typography")]
        public Font textFont;
        public Font titleFont;
        public float textFontSize = 24.0f;
        public float titleFontSize = 36.0f;

        private TMP_FontAsset generatedTMPTextFont;
        private TMP_FontAsset generatedTMPTitleFont;

        [Header("Button States (StyleBoxes)")]
        public ModularStyleBox buttonNormal = new ModularStyleBox { backgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) };
        public ModularStyleBox buttonHovered = new ModularStyleBox { backgroundColor = new Color(0.7f, 0.7f, 0.7f, 1.0f) };
        public ModularStyleBox buttonPressed = new ModularStyleBox { backgroundColor = new Color(0.4f, 0.4f, 0.4f, 1.0f) };
        public ModularStyleBox buttonDisabled = new ModularStyleBox { backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1.0f) };

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