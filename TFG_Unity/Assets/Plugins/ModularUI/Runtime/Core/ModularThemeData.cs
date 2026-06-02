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

        [Header("Background Settings")]
        public ModularStyleBox background = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);

        [Header("Typography")]
        public TMP_FontAsset textFont;
        public TMP_FontAsset titleFont;
        [SerializeField] private float textFontSize = 24.0f;
        public float TextFontSize
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UIConfiguration config = Resources.Load<UIConfiguration>("UIConfiguration");
                    if (config == null)
                    {
                        config = UnityEditor.AssetDatabase.LoadAssetAtPath<UIConfiguration>(ModularUIPaths.SettingsConfigPath);
                    }
                    if (config != null && config.selectedPlatform == UIConfiguration.TargetPlatform.MobilePortrait)
                    {
                        return 18.0f;
                    }
                    return textFontSize;
                }
#endif

                if (ModularThemeManager.HasInstance && ModularThemeManager.Instance != null)
                {
                    var config = ModularThemeManager.Instance.Config;
                    if (config != null && config.selectedPlatform == UIConfiguration.TargetPlatform.MobilePortrait)
                    {
                        return 18.0f;
                    }
                }
                return textFontSize;
            }
            set => textFontSize = value;
        }
        public float titleFontSize = 36.0f;
        public Color textColor = new Color(0.9f, 0.9f, 0.9f, 1.0f);
        public Color titleColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        public TMP_FontAsset GetTextFont()
        {
            return textFont;
        }

        public TMP_FontAsset GetTitleFont()
        {
            return titleFont;
        }

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

        private void OnValidate()
        {
            #if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isUpdating || UnityEditor.EditorApplication.isCompiling) return;
            #endif
            
            OnThemeChanged?.Invoke();
        }
    }
}