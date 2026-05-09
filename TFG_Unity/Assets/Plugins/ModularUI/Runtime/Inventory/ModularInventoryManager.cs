using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace ModularUIRuntime
{
    [Serializable]
    public class ItemEvent : UnityEvent<ItemData> { }

    public class ModularInventoryManager : MonoBehaviour
    {
        [Header("Detail Panel References")]
        public Image detailIcon;
        public TextMeshProUGUI detailName;
        public TextMeshProUGUI detailDescription;
        public TextMeshProUGUI useButtonText;
        public Button useButton;
        public Button dropButton;

        [Header("Selection Visual")]
        public RectTransform selectionFrame;

        [Header("Action Events")]
        public ItemEvent OnItemUsed;
        public ItemEvent OnItemDropped;

        private ModularInventoryItem currentSelectedItem;

        private void Awake()
        {
            if (selectionFrame != null)
            {
                selectionFrame.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            Canvas.ForceUpdateCanvases();
            Invoke(nameof(SelectFirstAvailableItem), 0.1f);
        }

        public void OnItemClicked(ModularInventoryItem item)
        {
            if (item.data == null) return;

            currentSelectedItem = item;

            if (detailIcon != null) detailIcon.sprite = item.data.itemIcon;
            if (detailName != null) detailName.text = item.data.itemName;
            if (detailDescription != null) detailDescription.text = item.data.itemDescription;

            UpdateActionButtons(item.data);

            if (selectionFrame != null)
            {
                selectionFrame.gameObject.SetActive(true);
                selectionFrame.SetParent(item.transform.parent);
                selectionFrame.anchoredPosition = Vector2.zero;
                selectionFrame.SetAsFirstSibling();
            }
        }

        private void UpdateActionButtons(ItemData data)
        {
            if (dropButton != null)
            {
                dropButton.interactable = data.canDrop;
            }

            if (useButton != null && useButtonText != null)
            {
                if (data.type == ItemType.Quest)
                {
                    useButton.interactable = false;
                    useButtonText.text = "USE";
                }
                else
                {
                    useButton.interactable = true;

                    if (!string.IsNullOrEmpty(data.customUseText))
                    {
                        useButtonText.text = data.customUseText;
                    }
                    else
                    {
                        useButtonText.text = (data.type == ItemType.Equipment) ? "EQUIP" : "USE";
                    }
                }
            }
        }

        public void UseSelectedItem()
        {
            if (currentSelectedItem != null && currentSelectedItem.data != null)
            {
                OnItemUsed.Invoke(currentSelectedItem.data);
            }
        }

        public void DropSelectedItem()
        {
            if (currentSelectedItem != null && currentSelectedItem.data != null)
            {
                if (!currentSelectedItem.data.canDrop) return;

                OnItemDropped.Invoke(currentSelectedItem.data);

                if (currentSelectedItem.data.isStackable && currentSelectedItem.currentQuantity > 1)
                {
                    currentSelectedItem.currentQuantity--;
                    currentSelectedItem.RefreshVisual();
                }
                else
                {
                    Destroy(currentSelectedItem.gameObject);

                    if (selectionFrame != null) selectionFrame.gameObject.SetActive(false);
                    currentSelectedItem = null;

                    if (detailName != null) detailName.text = "";
                    if (detailDescription != null) detailDescription.text = "";
                    if (detailIcon != null) detailIcon.sprite = null;
                }
            }
        }

        private void SelectFirstAvailableItem()
        {
            ModularInventoryItem firstItem = GetComponentInChildren<ModularInventoryItem>();
            if (firstItem != null) OnItemClicked(firstItem);
        }
    }
}