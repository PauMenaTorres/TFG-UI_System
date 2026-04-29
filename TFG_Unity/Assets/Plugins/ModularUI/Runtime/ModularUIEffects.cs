using UnityEngine;
using UnityEngine.EventSystems;

namespace ModularUIRuntime
{
    public class ModularUIEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("ANIMATION SETTINGS")]
        public float hoverScale = 1.05f;
        public float clickScale = 0.95f;
        public float transitionSpeed = 15f;

        private Vector3 targetScale = Vector3.one;
        private Vector3 originalScale;

        private void Awake()
        {
            originalScale = transform.localScale;
        }

        private void OnDisable()
        {
            transform.localScale = originalScale;
            targetScale = originalScale;
        }

        private void Update()
        {
            if (transform.localScale != targetScale)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * transitionSpeed);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            targetScale = originalScale * hoverScale;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            targetScale = originalScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            targetScale = originalScale * clickScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            targetScale = originalScale * hoverScale;
        }
    }
}