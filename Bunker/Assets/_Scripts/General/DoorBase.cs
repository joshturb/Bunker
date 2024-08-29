using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Core
{
    [Serializable]
    public struct DoorData
    {
        public Transform leftDoorTransform;
        public Transform rightDoorTransform;
        public Vector3 leftOpenPos;
        public Vector3 rightOpenPos;
        public Vector3 leftClosePos;
        public Vector3 rightClosePos;
        public AnimationCurve movementCurve;
        public float moveDuration;
    }
    public abstract class DoorBase : NetworkBehaviour
    {
        internal NetworkVariable<bool> isMoving = new(false);
        internal NetworkVariable<bool> openState = new(false);
        internal NetworkVariable<bool> isLocked = new(false);
        private float moveTimer;

        protected void MoveDoor(DoorData doorData, bool stateOverride = false, bool desiredState = false)
        {
            StartCoroutine(DoMove(doorData, stateOverride, desiredState));
        }

        private IEnumerator DoMove(DoorData doorData, bool stateOverride = false, bool desiredState = false)
        {
            SetIsMovingRpc(true);

            // Determine the desired state based on the stateOverride
            bool targetOpenState = stateOverride ? desiredState : !openState.Value;

            // Determine the start and end positions for both doors based on the targetOpenState
            Vector3 leftStartPosition = openState.Value ? doorData.leftOpenPos : doorData.leftClosePos;
            Vector3 leftEndPosition = targetOpenState ? doorData.leftOpenPos : doorData.leftClosePos;

            // Initialize variables for the right door if it's not null
            Vector3 rightStartPosition = Vector3.zero;
            Vector3 rightEndPosition = Vector3.zero;
            bool moveRightDoor = doorData.rightDoorTransform != null;

            if (moveRightDoor)
            {
                rightStartPosition = openState.Value ? doorData.rightOpenPos : doorData.rightClosePos;
                rightEndPosition = targetOpenState ? doorData.rightOpenPos : doorData.rightClosePos;
            }

            moveTimer = 0f;

            while (moveTimer <= doorData.moveDuration)
            {
                moveTimer += Time.deltaTime;
                float percentComplete = Mathf.Clamp01(moveTimer / doorData.moveDuration);
                float curveValue = doorData.movementCurve.Evaluate(percentComplete);

                // Move the left door
                doorData.leftDoorTransform.localPosition = Vector3.Lerp(leftStartPosition, leftEndPosition, curveValue);

                // Move the right door if it's not null
                if (moveRightDoor)
                {
                    doorData.rightDoorTransform.localPosition = Vector3.Lerp(rightStartPosition, rightEndPosition, curveValue);
                }

                yield return null;
            }

            // Ensure the left door ends exactly at the target position
            doorData.leftDoorTransform.localPosition = leftEndPosition;

            // Ensure the right door ends exactly at the target position if it's not null
            if (moveRightDoor)
            {
                doorData.rightDoorTransform.localPosition = rightEndPosition;
            }

            // Update the open state to reflect the target state after movement
            openState.Value = targetOpenState;
            SetOpenStateRpc(openState.Value);
            SetIsMovingRpc(false);
        }


        [Rpc(SendTo.Server)]
        public void SetIsMovingRpc(bool i)
        {
            isMoving.Value = i;
        }

        [Rpc(SendTo.Server)]
        public void SetOpenStateRpc(bool i)
        {
            openState.Value = i;
        }

        [Rpc(SendTo.Server)]
        public void LockDoorRpc()
        {
            isLocked.Value = true;
        }

        [Rpc(SendTo.Server)]
        public void UnlockDoorRpc()
        {
            isLocked.Value = false;
        }
    }
}
