using Core;
using Unity.Netcode;
using UnityEngine;

public class InteractTunnel : MonoBehaviour, IInteractable
{
    public Transform tunnelTransform;
    public void Interact<T>(RaycastHit hit, NetworkObject player, T type = default)
    {
        if (tunnelTransform.TryGetComponent(out IInteractableTunnel tunnel))
        {
            tunnel.OnInteractedRpc();
        }
    }
}
