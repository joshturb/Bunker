using System.Collections;
using Unity.Netcode;
using UnityEngine;

public static class NetworkUtils
{
    public static bool TryGetNetworkObjectReferenceFromTransform(Transform transform, out NetworkObjectReference networkObjectReference)
    {
        return TryGetNetworkObjectReferenceFromComponent(transform, out networkObjectReference);
    }

    public static bool TryGetNetworkObjectReferenceFromCollider(Collider collider, out NetworkObjectReference networkObjectReference)
    {
        return TryGetNetworkObjectReferenceFromComponent(collider, out networkObjectReference);
    }

    public static bool TryGetNetworkObjectReferenceFromGameObject(GameObject gameObject, out NetworkObjectReference networkObjectReference)
    {
        return TryGetNetworkObjectReferenceFromComponent(gameObject.transform, out networkObjectReference);
    }

    private static bool TryGetNetworkObjectReferenceFromComponent(Component component, out NetworkObjectReference networkObjectReference)
    {
        if (component.TryGetComponent(out NetworkObject networkObject))
        {
            networkObjectReference = new NetworkObjectReference(networkObject);
            return true;
        }

        Debug.LogError("[ FuncUtils ] : Failed to get NetworkObjectReference From Component");
        
        networkObjectReference = default;
        return false;
    }

    public static bool TryFindComponentFromTransform<T>(Transform transform, out T component) where T : Component
    {
        // Attempt to get the component from the transform
        if (transform.TryGetComponent(out component))
        {
            return true;
        }

        // Attempt to get the component from the parent
        if (transform.parent != null && transform.parent.TryGetComponent(out component))
        {
            return true;
        }

        // Attempt to get the component from the children
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out component))
            {
                return true;
            }
        }

        // Component not found
        component = null;
        return false;
    }

    [Rpc(SendTo.Server)]
    public static void TeleportClientFromServer(NetworkObjectReference networkObjectReference, Vector3 position)
    {
        if (!networkObjectReference.TryGet(out NetworkObject networkObject))
            return;
        if (!networkObject.TryGetComponent(out ClientNetworkTransform clientNetworkTransform))
            return;
        if (!networkObject.TryGetComponent(out CharacterController characterController))
            return;

        // Temporarily disable character controller;
        characterController.enabled = false;

        // Temporarily give the server authority
        clientNetworkTransform.authorityMode = AuthorityMode.Server;

        // Teleport the player
        clientNetworkTransform.Teleport(position, Quaternion.identity, Vector3.one);

        // Start coroutine to reset authority
        clientNetworkTransform.authorityMode = AuthorityMode.Client;

        characterController.enabled = true;
    }
}
