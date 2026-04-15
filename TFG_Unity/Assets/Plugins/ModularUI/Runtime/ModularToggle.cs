using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ModularUIRuntime
{
    [RequireComponent(typeof(Toggle))]
    public class ModularToggle : ModularComponents
    {
        [Header("Toggle Settings")]
        [SerializeField] private bool isOn = false;
        [SerializeField] private bool showLabel = true;
        [SerializeField] private string labelText = "Toggle Option";
        [SerializeField] private float labelFontSize = 24.0f;
        [SerializeField] private Color labelColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        [Header("Overrides")]
        [SerializeField] private ModularStyleBox overrideBackground = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);
        [SerializeField] private ModularStyleBox overrideCheckmark = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);
        [SerializeField] private float overrideFontSize = 24.0f;
        [SerializeField] private Color overrideColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        private Toggle targetToggle;
        private Image backgroundImage;
        private Image checkmarkImage;
        private TextMeshProUGUI labelComponent;
        private bool lastOverrideState;

        protected override void OnValidate()
        {
            FetchReferences();

            if (targetToggle != null)
            {
                targetToggle.isOn = isOn;
            }

            if (labelComponent != null)
            {
                labelComponent.gameObject.SetActive(showLabel);
                labelComponent.text = labelText;

                if (useOverride)
                {
                    labelComponent.fontSize = overrideFontSize;
                    labelComponent.color = overrideColor;
                }

                if (!useOverride)
                {
                    labelComponent.fontSize = labelFontSize;
                    labelComponent.color = labelColor;
                }
            }

            if (useOverride && lastOverrideState == false)
            {
                if (currentTheme != null)
                {
                    overrideBackground = currentTheme.toggleBackground;
                    overrideCheckmark = currentTheme.toggleCheckmark;
                    overrideFontSize = labelFontSize;
                    overrideColor = labelColor;
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

            ModularStyleBox activeBG = useOverride ? overrideBackground : currentTheme.toggleBackground;
            ModularStyleBox activeCheck = useOverride ? overrideCheckmark : currentTheme.toggleCheckmark;

            ApplyStyle(backgroundImage, activeBG);
            ApplyStyle(checkmarkImage, activeCheck);

            if (labelComponent != null)
            {
                if (useOverride == false)
                {
                    labelComponent.font = currentTheme.GetTextFont();
                    labelComponent.fontSize = labelFontSize;
                    labelComponent.color = labelColor;
                }

                if (useOverride == true)
                {
                    labelComponent.fontSize = overrideFontSize;
                    labelComponent.color = overrideColor;
                }

                labelComponent.SetAllDirty();
            }
        }

        private void FetchReferences()
        {
            if (targetToggle == null)
            {
                targetToggle = GetComponent<Toggle>();
            }

            if (backgroundImage == null)
            {
                Transform bgTransform = transform.Find("Background");

                if (bgTransform != null)
                {
                    backgroundImage = bgTransform.GetComponent<Image>();
                }
            }

            if (targetToggle != null && targetToggle.graphic != null)
            {
                checkmarkImage = targetToggle.graphic.GetComponent<Image>();
            }

            if (labelComponent == null)
            {
                labelComponent = GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }
}