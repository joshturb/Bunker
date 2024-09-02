using System;
using Core;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Item : NetworkBehaviour, IInteractable
{
    [SerializeField] private InventoryItemData itemData;
    public int itemAmount;
    public UnityAction<IInteractable> OnInteractionComplete { get; set; }

    public void Interact(ReferenceHub interactor, RaycastHit hit, out bool interactSuccessful)
    {
        HandleInteraction(interactor);
        interactSuccessful = true;
    }

    private void HandleInteraction(ReferenceHub interactor)
    {
        if (interactor.inventory.AddToInventory(itemData, itemAmount))
        {
            Destroy(gameObject);
        }
    }
}
