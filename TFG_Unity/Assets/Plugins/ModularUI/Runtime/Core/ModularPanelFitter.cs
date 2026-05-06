using UnityEngine;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class ModularPanelFitter : MonoBehaviour
    {
        private RectTransform rectTransform;

        private void OnEnable()
        {
            FitToParent();
        }

        private void OnTransformParentChanged()
        {
            FitToParent();
        }

        public void FitToParent()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            if (rectTransform.parent == null)
            {
                return;
            }

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;

            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            rectTransform.localScale = Vector3.one;

            Vector3 lp = rectTransform.localPosition;
            rectTransform.localPosition = new Vector3(lp.x, lp.y, 0f);
        }
    }
}