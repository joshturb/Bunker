using UnityEngine.Events;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[System.Serializable]
public class InventorySystem
{
	[SerializeField] private List<InventorySlot> inventorySlots;
	public List<InventorySlot> InventorySlots => inventorySlots;
	public int InventorySize => InventorySlots.Count;

	public UnityAction<InventorySlot> OnInventorySlotChanged;

	public InventorySystem(int size)
	{
		inventorySlots = new(size);

		for (int i = 0; i < size; i++)
		{
			inventorySlots.Add(new InventorySlot());
		}
	}

	public bool AddToInventory(InventoryItemData itemToAdd, int amountToAdd)
	{
		if (ContainsItem(itemToAdd, out List<InventorySlot> invSlot)) // check if item exists in inventory.
		{
			foreach (var slot in invSlot)
			{
				if(slot.RoomLeftInStack(amountToAdd))
				{
					slot.AddToStack(amountToAdd);
					OnInventorySlotChanged?.Invoke(slot);
					return true;
				}
			}
		}
		else if (HasFreeSlot(out InventorySlot freeSlot)) // gets first free slot
		{
			freeSlot.UpdateInventorySlot(itemToAdd, amountToAdd);
			OnInventorySlotChanged?.Invoke(freeSlot);
			return true;
		}
		return false;
	}

	public bool ContainsItem(InventoryItemData itemToAdd, out List<InventorySlot> invSlot)
	{
		invSlot = InventorySlots.Where(i => i.ItemData == itemToAdd).ToList();
		return invSlot != null;
	}

	public bool HasFreeSlot(out InventorySlot freeSlot)
	{
		freeSlot = InventorySlots.FirstOrDefault(i => i.ItemData == null);
		return freeSlot != null;
	}
}
