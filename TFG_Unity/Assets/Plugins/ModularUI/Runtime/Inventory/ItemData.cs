using UnityEngine;

namespace ModularUIRuntime
{
    public enum ItemType
    {
        Consumable,
        Equipment,
        Quest,
        Trash
    }

    [CreateAssetMenu(fileName = "NewItem", menuName = "Modular UI/Inventory/Item Data")]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        public Sprite itemIcon;
        [TextArea(3, 5)]
        public string itemDescription;
        public ItemType type;
        public bool isStackable;
        public bool canDrop;
        [Tooltip("Si se deja vacío, el sistema usará EQUIP para equipo y USE para el resto.")]
        public string customUseText;
    }
}