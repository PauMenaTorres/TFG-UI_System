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
        [Tooltip("If left empty, the system will use EQUIP for equipment and USE for everything else.")]
        public string customUseText;
    }
}