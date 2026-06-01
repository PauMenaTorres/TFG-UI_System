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

            // Adjust size in mobile portrait dynamically to prevent items from being too large for the slots
            if (rectTransform != null && ModularThemeManager.HasInstance && ModularThemeManager.Instance != null)
            {
                var config = ModularThemeManager.Instance.Config;
                if (config != null && config.selectedPlatform == UIConfiguration.TargetPlatform.MobilePortrait)
                {
                    rectTransform.sizeDelta = new Vector2(32f, 32f);
                }
            }

            RefreshVisual();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null)
                {
                    RefreshVisual();
                }
            };
        }
#endif

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

            if (data != null && iconImage != null)
            {
                if (iconImage.sprite != data.itemIcon)
                {
                    iconImage.sprite = data.itemIcon;
                }

                if (quantityText != null)
                {
                    bool targetActive = data.isStackable && currentQuantity > 1;
                    if (quantityText.gameObject.activeSelf != targetActive)
                    {
                        quantityText.gameObject.SetActive(targetActive);
                    }

                    if (targetActive)
                    {
                        string targetText = "x" + currentQuantity;
                        if (quantityText.text != targetText)
                        {
                            quantityText.text = targetText;
                        }
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
            transform.position = eventData.position;
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