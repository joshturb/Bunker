using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public struct ElevatorData
{
    public float elevatorTime;
    public float timeToOpen;
    public List<NetworkObject> playersInElevator;
    public Door door;
}

public class Elevator : NetworkBehaviour, IInteractableTunnel
{
    [SerializeField] private ElevatorData elevatorData;
    [SerializeField] private Vector3 teleportPosition;

    [Rpc(SendTo.Server)]
    public void OnInteractedRpc()
    {
        StartCoroutine(StartElevator());
    }

    private IEnumerator StartElevator()
    {
        elevatorData.door.SetDoorStateRpc(false);

        yield return new WaitForSeconds(elevatorData.elevatorTime);

        foreach (var player in elevatorData.playersInElevator)
        {
            Vector3 position = new(player.transform.position.x + teleportPosition.x, teleportPosition.y, player.transform.position.z + teleportPosition.z);
            NetworkUtils.TeleportClientFromServer(player, position);
        }
        yield return new WaitForSeconds(elevatorData.timeToOpen);

        elevatorData.door.SetDoorStateRpc(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && NetworkUtils.TryGetNetworkObjectReferenceFromCollider(other, out NetworkObjectReference reference))
        {
            AddPlayerToElevatorListRpc(reference);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && NetworkUtils.TryGetNetworkObjectReferenceFromCollider(other, out NetworkObjectReference reference))
        {
            RemovePlayerToElevatorListRpc(reference);
        }
    }

    [Rpc(SendTo.Server)]
    public void AddPlayerToElevatorListRpc(NetworkObjectReference reference)
    {
        if (elevatorData.playersInElevator.Contains(reference))
            return;

        elevatorData.playersInElevator.Add(reference);
            
    }
    [Rpc(SendTo.Server)]
    public void RemovePlayerToElevatorListRpc(NetworkObjectReference reference)
    {
        if (!elevatorData.playersInElevator.Contains(reference))
            return;

        elevatorData.playersInElevator.Remove(reference);
    }
}
