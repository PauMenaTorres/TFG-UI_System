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

        private bool Approximately(float a, float b)
        {
            return Mathf.Abs(a - b) < 0.005f;
        }

        private bool Approximately(Vector2 a, Vector2 b)
        {
            return Mathf.Abs(a.x - b.x) < 0.005f &&
                   Mathf.Abs(a.y - b.y) < 0.005f;
        }

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

                    if (rectTransform != null)
                    {
                        if (!Approximately(size, rectTransform.sizeDelta))
                        {
                            size = rectTransform.sizeDelta;
                            UnityEditor.EditorUtility.SetDirty(this);
                        }
                    }

                    if (targetSlider != null)
                    {
                        if (!Approximately(targetSlider.value, sliderValue))
                        {
                            targetSlider.value = sliderValue;
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

            if (targetSlider == null)
            {
                targetSlider = GetComponent<Slider>();
            }

            if (targetSlider == null || currentTheme == null)
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