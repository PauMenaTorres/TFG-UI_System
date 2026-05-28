using UnityEngine;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    [RequireComponent(typeof(Image))]
    public class ModularBackground : ModularComponents
    {
        [Header("Overrides")]
        [SerializeField] private ModularStyleBox overrideBackground = new ModularStyleBox(ModularStyleBox.StyleBoxType.SolidColor);

        private Image targetImage;

        protected override void Awake()
        {
            targetImage = GetComponent<Image>();
            base.Awake();
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

            ModularStyleBox activeBG = useOverride ? overrideBackground : currentTheme.background;
            ApplyStyle(targetImage, activeBG);
        }
    }
}
