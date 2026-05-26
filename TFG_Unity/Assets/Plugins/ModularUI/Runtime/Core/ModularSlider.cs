using UnityEngine;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    [RequireComponent(typeof(Slider))]
    public class ModularSlider : ModularComponents
    {
        [Header("Slider Settings")]
        [SerializeField][Range(0.0f, 1.0f)] private float sliderValue = 0.5f;
        [SerializeField] private Vector2 size = new Vector2(160f, 20f);

        [Header("Overrides")]
        [SerializeField] private ModularStyleBox overrideBackground = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);
        [SerializeField] private ModularStyleBox overrideFill = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);
        [SerializeField] private ModularStyleBox overrideHandle = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);

        private Slider targetSlider;
        private RectTransform rectTransform;

        protected override void OnValidate()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            if (targetSlider == null)
            {
                targetSlider = GetComponent<Slider>();
            }

            base.OnValidate();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (this == null) return;
                    bool changed = false;

                    if (rectTransform != null)
                    {
                        if (rectTransform.sizeDelta != size)
                        {
                            rectTransform.sizeDelta = size;
                            changed = true;
                        }
                    }

                    if (targetSlider != null)
                    {
                        if (targetSlider.value != sliderValue)
                        {
                            targetSlider.value = sliderValue;
                            changed = true;
                        }
                    }

                    if (changed)
                    {
                        MarkAsDirty(rectTransform);
                        MarkAsDirty(targetSlider);
                        MarkAsDirty(this);
                    }
                };
            }
#endif
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (targetSlider == null)
            {
                targetSlider = GetComponent<Slider>();
            }

            if (currentTheme == null)
            {
                return;
            }

            ModularStyleBox activeBG = useOverride ? overrideBackground : currentTheme.sliderBackground;
            ModularStyleBox activeFill = useOverride ? overrideFill : currentTheme.sliderFill;
            ModularStyleBox activeHandle = useOverride ? overrideHandle : currentTheme.sliderHandle;

            if (targetSlider.fillRect != null)
            {
                Image fillImage = targetSlider.fillRect.GetComponent<Image>();

                if (fillImage != null)
                {
                    ApplyStyle(fillImage, activeFill);
                }
            }

            if (targetSlider.handleRect != null)
            {
                Image handleImage = targetSlider.handleRect.GetComponent<Image>();

                if (handleImage != null)
                {
                    ApplyStyle(handleImage, activeHandle);
                }
            }

            Image backgroundImage = null;

            foreach (Transform child in transform)
            {
                if (child.name == "Background")
                {
                    backgroundImage = child.GetComponent<Image>();
                    break;
                }
            }

            if (backgroundImage != null)
            {
                ApplyStyle(backgroundImage, activeBG);
            }
        }
    }
}