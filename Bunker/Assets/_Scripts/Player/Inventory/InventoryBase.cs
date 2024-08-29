using Unity.Netcode;
using InventorySystem.Items;
using System;

namespace InventorySystem
{
    public struct InventorySlot
    {
        public ItemBase item;
        public int amount;
    }
    public abstract class InventoryBase : NetworkBehaviour
    {
        public int inventorySlotAmount = 10;
        public InventorySlot[] inventorySlots;

        public event Action<InventorySlot> OnAddedItem;
        public event Action<InventorySlot> OnRemoveItem;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            inventorySlots = new InventorySlot[inventorySlotAmount];
            for (int i = 0; i < inventorySlotAmount; i++)
            {
                inventorySlots[i] = new InventorySlot { item = null, amount = 0 };
            }
        }

        public bool AddItem(ItemBase item, int amount)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].item == item)
                {
                    inventorySlots[i].amount += amount;
                    OnAddedItem?.Invoke(inventorySlots[i]);
                    return true;
                }
            }
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].item == null)
                {
                    inventorySlots[i] = new InventorySlot { item = item, amount = amount };
                    OnAddedItem?.Invoke(inventorySlots[i]);
                    return true;
                }
            }

            return false; // Inventory is full
        }

        public bool RemoveItem(ItemBase item, int amount)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].item == item && inventorySlots[i].amount >= amount)
                {
                    inventorySlots[i].amount -= amount;
                    if (inventorySlots[i].amount <= 0)
                    {
                        inventorySlots[i] = new InventorySlot { item = null, amount = 0 };
                    }
                    OnRemoveItem?.Invoke(inventorySlots[i]);
                    return true;
                }
            }

            return false; // Item not found or insufficient amount
        }

        public InventorySlot GetItem(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < inventorySlots.Length)
            {
                return inventorySlots[slotIndex];
            }

            return default;
        }

        public bool HasItem(ItemBase item, int amount)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.item == item && slot.amount >= amount)
                {
                    return true;
                }
            }
            return false;
        }
    }
}