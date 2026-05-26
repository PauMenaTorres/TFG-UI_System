using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    [ExecuteAlways]
    [RequireComponent(typeof(Image))]
    public class ModularInventorySlot : ModularComponents, IDropHandler
    {
        private Image slotImage;

        protected override void Awake()
        {
            slotImage = GetComponent<Image>();
            base.Awake();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
        }

        public override void ApplyTheme()
        {
            base.ApplyTheme();

            if (slotImage == null)
            {
                slotImage = GetComponent<Image>();
            }

            if (slotImage != null && currentTheme != null)
            {
                Color targetColor;
                
                if (currentTheme.background.backgroundType == ModularStyleBox.StyleBoxType.SolidColor)
                {
                    targetColor = currentTheme.background.backgroundColor;
                }
                else if (currentTheme.background.backgroundType == ModularStyleBox.StyleBoxType.Sprite)
                {
                    targetColor = currentTheme.background.tintColor;
                }
                else
                {
                    targetColor = currentTheme.primaryColor;
                }

                targetColor.a = 1.0f;

                if (slotImage.color != targetColor)
                {
                    slotImage.color = targetColor;
                    MarkAsDirty(slotImage);
                    MarkAsDirty(this);
                }
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            GameObject dropped = eventData.pointerDrag;
            ModularInventoryItem draggedItem = dropped.GetComponent<ModularInventoryItem>();

            if (draggedItem != null)
            {
                if (transform.childCount == 0)
                {
                    draggedItem.parentAfterDrag = transform;
                }
                else
                {
                    ModularInventoryItem existingItem = transform.GetChild(0).GetComponent<ModularInventoryItem>();

                    if (existingItem != null)
                    {
                        existingItem.transform.SetParent(draggedItem.parentAfterDrag, false);
                        draggedItem.parentAfterDrag = transform;

                        RectTransform existingRect = existingItem.GetComponent<RectTransform>();

                        if (existingRect != null)
                        {
                            existingRect.anchorMin = new Vector2(0.5f, 0.5f);
                            existingRect.anchorMax = new Vector2(0.5f, 0.5f);
                            existingRect.pivot = new Vector2(0.5f, 0.5f);
                            existingRect.anchoredPosition = Vector2.zero;
                        }
                    }
                }
            }
        }
    }
}