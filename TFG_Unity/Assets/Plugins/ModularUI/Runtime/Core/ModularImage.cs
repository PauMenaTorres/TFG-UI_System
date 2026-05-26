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

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (targetImage == null)
            {
                targetImage = GetComponent<Image>();
            }

            if (currentTheme == null)
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
                if (targetImage.color != overrideColor)
                {
                    targetImage.color = overrideColor;
                    changed = true;
                }
            }

            if (!useOverride)
            {
                Color targetColor = Color.white;

                if (colorRole == ImageColorRole.Primary)
                {
                    targetColor = currentTheme.primaryColor;
                }

                if (colorRole == ImageColorRole.Secondary)
                {
                    targetColor = currentTheme.secondaryColor;
                }

                if (colorRole == ImageColorRole.Custom)
                {
                    targetColor = customColor;
                }

                if (targetImage.color != targetColor)
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