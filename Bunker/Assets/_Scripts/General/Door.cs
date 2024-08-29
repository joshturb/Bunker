using Unity.Netcode;
using UnityEngine;

namespace Core
{
    public class Door : DoorBase, IInteractableTunnel
    {
        public DoorData doorData;

        [ContextMenu("Interact")]

        [Rpc(SendTo.Server)]
        public void OnInteractedRpc()
        {
            if (CanMove())
            {
                MoveDoor(doorData);
            }
        }
        [Rpc(SendTo.Server)]
        public void SetDoorStateRpc(bool state)
        {
            if (CanMove())
            {
                MoveDoor(doorData, true, state);
            }
        }
    
        private bool CanMove()
        {
            return !isMoving.Value && !isLocked.Value;
        }
    }
}