using Core;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class InteractTunnel : MonoBehaviour, IInteractable
{
    public Transform tunnelTransform;
    public UnityAction<IInteractable> OnInteractionComplete {get; set;}

    public void Interact(ReferenceHub interactor, RaycastHit hit)
    {
        if (tunnelTransform.TryGetComponent(out IInteractableTunnel tunnel))
        {
            tunnel.OnInteractedRpc();
        }
    }

    public void Interact(ReferenceHub interactor, RaycastHit hit, out bool interactSuccessful)
    {
        throw new System.NotImplementedException();
    }
}
