using UnityEngine;
using UnityEngine.EventSystems;

namespace ModularUIRuntime
{
    public class ModularInventorySlot : MonoBehaviour, IDropHandler
    {
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