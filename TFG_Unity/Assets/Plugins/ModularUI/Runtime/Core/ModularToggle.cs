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

        private bool Approximately(float a, float b)
        {
            return Mathf.Abs(a - b) < 0.005f;
        }

        private bool Approximately(Color a, Color b)
        {
            return Mathf.Abs(a.r - b.r) < 0.005f &&
                   Mathf.Abs(a.g - b.g) < 0.005f &&
                   Mathf.Abs(a.b - b.b) < 0.005f &&
                   Mathf.Abs(a.a - b.a) < 0.005f;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (this == null) return;
                    FetchReferences();

                    if (targetToggle != null)
                    {
                        if (targetToggle.isOn != isOn)
                        {
                            targetToggle.isOn = isOn;
                        }
                    }

                    if (labelComponent != null)
                    {
                        if (labelComponent.gameObject.activeSelf != showLabel)
                        {
                            labelComponent.gameObject.SetActive(showLabel);
                        }

                        if (labelComponent.text != labelText)
                        {
                            labelComponent.text = labelText;
                        }
                    }
                };
            }
#endif
        }

        public override void ApplyTheme()
        {
#if UNITY_EDITOR
            if (!gameObject.scene.IsValid())
            {
                var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject);
                if (stage == null)
                {
                    return;
                }
            }
#endif

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
                    var targetFont = currentTheme.GetTextFont();
                    if (targetFont != null && labelComponent.font != targetFont)
                    {
                        labelComponent.font = targetFont;
                        labelChanged = true;
                    }

                    if (!Approximately(labelComponent.fontSize, labelFontSize))
                    {
                        labelComponent.fontSize = labelFontSize;
                        labelChanged = true;
                    }

                    if (!Approximately(labelComponent.color, labelColor))
                    {
                        labelComponent.color = labelColor;
                        labelChanged = true;
                    }
                }
                else
                {
                    if (!Approximately(labelComponent.fontSize, overrideFontSize))
                    {
                        labelComponent.fontSize = overrideFontSize;
                        labelChanged = true;
                    }

                    if (!Approximately(labelComponent.color, overrideColor))
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