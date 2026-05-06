using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    public class ModularInventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        public ItemData data;
        public int currentQuantity = 1;
        public Image iconImage;
        public TMPro.TextMeshProUGUI quantityText;

        [HideInInspector]
        public Transform parentAfterDrag;

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
            RefreshVisual();
        }

        private void OnValidate()
        {
            RefreshVisual();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!eventData.dragging)
            {
                ModularInventoryManager manager = GetComponentInParent<ModularInventoryManager>();

                if (manager != null)
                {
                    manager.OnItemClicked(this);
                }
            }
        }

        public void RefreshVisual()
        {
            if (data != null && iconImage != null)
            {
                iconImage.sprite = data.itemIcon;

                if (quantityText != null)
                {
                    if (data.isStackable && currentQuantity > 1)
                    {
                        quantityText.gameObject.SetActive(true);
                        quantityText.text = "x" + currentQuantity;
                    }
                    else
                    {
                        quantityText.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.SetParent(parentAfterDrag, false);

            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
            }

            canvasGroup.blocksRaycasts = true;

            ModularInventoryManager manager = GetComponentInParent<ModularInventoryManager>();

            if (manager != null)
            {
                manager.OnItemClicked(this);
            }
        }
    }
}