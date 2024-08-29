using InventorySystem.Items;

namespace InventorySystem
{
    public class Inventory : InventoryBase
    {
        public int selectedInventorySlot;
        public InventorySlot equippedItemSlot;

        public void ItemPickedUp(ItemBase itemBase, int amount)
        {
            AddItem(itemBase, amount);
        }
    }
}