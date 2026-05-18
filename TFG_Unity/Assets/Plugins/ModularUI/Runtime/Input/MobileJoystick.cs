using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ModularUIRuntime
{
    public class MobileJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        public RectTransform background;
        public RectTransform handle;
        public MobileTouchInput inputProvider;
        public float range = 0.5f;

        private Vector2 inputVector;
        private Vector2 startPosition;

        private void Start()
        {
            startPosition = background.anchoredPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out pos))
            {
                float radius = background.rect.width / 2f;
                inputVector = pos / radius;
                inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

                handle.anchoredPosition = inputVector * (radius * range);
                inputProvider.UpdateMovement(inputVector);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            inputVector = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
            background.anchoredPosition = startPosition;
            inputProvider.UpdateMovement(Vector2.zero);
        }
    }
}
