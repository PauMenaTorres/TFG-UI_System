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

        protected override void OnValidate()
        {
            FetchReferences();

            bool changed = false;

            if (targetToggle != null)
            {
                if (targetToggle.isOn != isOn)
                {
                    targetToggle.isOn = isOn;
                    changed = true;
                }
            }

            if (labelComponent != null)
            {
                if (labelComponent.gameObject.activeSelf != showLabel)
                {
                    labelComponent.gameObject.SetActive(showLabel);
                    changed = true;
                }

                if (labelComponent.text != labelText)
                {
                    labelComponent.text = labelText;
                    changed = true;
                }
            }

            if (changed)
            {
                MarkAsDirty(targetToggle);
                MarkAsDirty(labelComponent);
                MarkAsDirty(this);
            }

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
                bool labelChanged = false;
                if (useOverride == false)
                {
                    if (labelComponent.font != currentTheme.GetTextFont())
                    {
                        labelComponent.font = currentTheme.GetTextFont();
                        labelChanged = true;
                    }

                    if (labelComponent.fontSize != labelFontSize)
                    {
                        labelComponent.fontSize = labelFontSize;
                        labelChanged = true;
                    }

                    if (labelComponent.color != labelColor)
                    {
                        labelComponent.color = labelColor;
                        labelChanged = true;
                    }
                }

                if (useOverride == true)
                {
                    if (labelComponent.fontSize != overrideFontSize)
                    {
                        labelComponent.fontSize = overrideFontSize;
                        labelChanged = true;
                    }

                    if (labelComponent.color != overrideColor)
                    {
                        labelComponent.color = overrideColor;
                        labelChanged = true;
                    }
                }

                if (labelChanged)
                {
                    MarkAsDirty(labelComponent);
                    MarkAsDirty(this);
                }
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