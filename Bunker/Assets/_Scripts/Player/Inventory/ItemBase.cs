using Unity.Netcode;

namespace InventorySystem.Items
{
    public abstract class ItemBase : NetworkBehaviour
    {
		public ItemType ItemTypeId;

		public ItemCategory Category;

		public ItemTier TierFlags;

		public ReferenceHub Owner { get; internal set; }

		internal Inventory OwnerInventory => Owner.inventory; 

        public virtual bool AllowEquip => true;

        public bool IsEquipped {get; internal set; }

		public abstract float Weight { get; }

        public int Amount;

		public virtual void OnEquipped() { }

    }
}
