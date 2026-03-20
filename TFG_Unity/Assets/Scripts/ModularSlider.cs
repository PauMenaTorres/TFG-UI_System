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

        [Header("Styles")]
        [SerializeField] private ModularStyleBox backgroundStyle = new ModularStyleBox { backgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) };
        [SerializeField] private ModularStyleBox fillStyle = new ModularStyleBox { backgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) };
        [SerializeField] private ModularStyleBox handleStyle = new ModularStyleBox { backgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) };

        [SerializeField] private ModularStyleBox overrideBackground = new ModularStyleBox { backgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) };
        [SerializeField] private ModularStyleBox overrideFill = new ModularStyleBox { backgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) };
        [SerializeField] private ModularStyleBox overrideHandle = new ModularStyleBox { backgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f) };

        private Slider targetSlider;
        private RectTransform rectTransform;
        private bool lastOverrideState;

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

            if (rectTransform != null)
            {
                rectTransform.sizeDelta = size;
            }

            if (targetSlider != null)
            {
                targetSlider.value = sliderValue;
            }

            base.OnValidate();

            if (useOverride && lastOverrideState == false)
            {
                if (currentTheme != null)
                {
                    overrideBackground = backgroundStyle;
                    overrideFill = fillStyle;
                    overrideHandle = handleStyle;
                }
            }
            lastOverrideState = useOverride;
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

            ModularStyleBox activeBG = useOverride ? overrideBackground : backgroundStyle;
            ModularStyleBox activeFill = useOverride ? overrideFill : fillStyle;
            ModularStyleBox activeHandle = useOverride ? overrideHandle : handleStyle;

            if (targetSlider.fillRect != null)
            {
                ApplyStyle(targetSlider.fillRect.GetComponent<Image>(), activeFill);
            }

            if (targetSlider.handleRect != null)
            {
                ApplyStyle(targetSlider.handleRect.GetComponent<Image>(), activeHandle);
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
