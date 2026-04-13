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
        [Tooltip("The base sprite that this image will display")]
        [SerializeField] private Sprite baseSprite;

        [Tooltip("Prevent the image from stretching and distorting")]
        [SerializeField] private bool preserveAspect = true;

        [Header("Color Settings")]
        [Tooltip("What color from the theme should the image be tinted with?")]
        [SerializeField] private ImageColorRole colorRole = ImageColorRole.OriginalColor;

        [Tooltip("Custom color applied only if colorRole is set to Custom")]
        [SerializeField] private Color customColor = Color.white;

        [SerializeField] private Color overrideColor = Color.white;

        private Image targetImage;
        private bool lastOverrideState;

        protected override void Awake()
        {
            targetImage = GetComponent<Image>();
            base.Awake();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (useOverride && lastOverrideState == false)
            {
                if (currentTheme != null)
                {
                    if (colorRole == ImageColorRole.Primary)
                    {
                        overrideColor = currentTheme.primaryColor;
                    }
                    else if (colorRole == ImageColorRole.Secondary)
                    {
                        overrideColor = currentTheme.secondaryColor;
                    }
                    else if (colorRole == ImageColorRole.Custom)
                    {
                        overrideColor = customColor;
                    }
                    else
                    {
                        overrideColor = Color.white;
                    }
                }
            }

            lastOverrideState = useOverride;
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

            targetImage.sprite = baseSprite;
            targetImage.preserveAspect = preserveAspect;

            if (useOverride)
            {
                targetImage.color = overrideColor;
            }
            else
            {
                if (colorRole == ImageColorRole.Primary)
                {
                    targetImage.color = currentTheme.primaryColor;
                }
                else if (colorRole == ImageColorRole.Secondary)
                {
                    targetImage.color = currentTheme.secondaryColor;
                }
                else if (colorRole == ImageColorRole.Custom)
                {
                    targetImage.color = customColor;
                }
                else
                {
                    targetImage.color = Color.white;
                }
            }
        }
    }
}