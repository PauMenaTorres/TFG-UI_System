using UnityEngine;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    [RequireComponent(typeof(Image))]
    public class ModularImage : ModularComponents
    {
        public enum ImageColorRole
        {
            OriginalColor,
            Primary,
            Secondary,
            Custom
        }

        [Header("Image Settings")]
        [SerializeField] private Sprite baseSprite;
        [SerializeField] private bool preserveAspect = true;

        [Header("Color Settings")]
        [SerializeField] private ImageColorRole colorRole = ImageColorRole.OriginalColor;
        [SerializeField] private Color customColor = Color.white;
        [SerializeField] private Color overrideColor = Color.white;

        private Image targetImage;

        protected override void Awake()
        {
            targetImage = GetComponent<Image>();
            base.Awake();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
        }

        private bool Approximately(Color a, Color b)
        {
            return Mathf.Abs(a.r - b.r) < 0.005f &&
                   Mathf.Abs(a.g - b.g) < 0.005f &&
                   Mathf.Abs(a.b - b.b) < 0.005f &&
                   Mathf.Abs(a.a - b.a) < 0.005f;
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

            if (targetImage == null)
            {
                targetImage = GetComponent<Image>();
            }

            if (targetImage == null || currentTheme == null)
            {
                return;
            }

            bool changed = false;

            if (targetImage.sprite != baseSprite)
            {
                targetImage.sprite = baseSprite;
                changed = true;
            }

            if (targetImage.preserveAspect != preserveAspect)
            {
                targetImage.preserveAspect = preserveAspect;
                changed = true;
            }

            if (useOverride)
            {
                if (!Approximately(targetImage.color, overrideColor))
                {
                    targetImage.color = overrideColor;
                    changed = true;
                }
            }
            else
            {
                Color targetColor = Color.white;

                if (colorRole == ImageColorRole.Primary)
                {
                    targetColor = currentTheme.primaryColor;
                }
                else if (colorRole == ImageColorRole.Secondary)
                {
                    targetColor = currentTheme.secondaryColor;
                }
                else if (colorRole == ImageColorRole.Custom)
                {
                    targetColor = customColor;
                }

                if (!Approximately(targetImage.color, targetColor))
                {
                    targetImage.color = targetColor;
                    changed = true;
                }
            }

            if (changed)
            {
                MarkAsDirty(targetImage);
                MarkAsDirty(this);
            }
        }
    }
}