using System;
using UnityEngine;
using Unity.Netcode;
using Core;

public class Interaction : NetworkBehaviour
{
    [SerializeField] private float interactDelay = 1.0f; // Delay for interaction
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private int delayedInteractionLayer = 7; // Specific layer for delayed interaction
    [SerializeField] private float delayedInteractionTime = 0.5f;
    [SerializeField] private LayerMask layerMask; // All interactable layers
    public Camera mainCamera;
    public bool canInteract = true;

    private float interactTimer = 0f;
    private bool isInteractingWithDelayedObject = false;
    private bool hasInteractedDelayed = false;
    private InputManager inputManager;

    public event Action<RaycastHit> E_OnItemPickup;

    private void Start()
    {
        inputManager = InputManager.Instance;
    }

    private void LateUpdate()
    {
        if (!IsOwner || !canInteract) return;

        if (inputManager.InteractedThisFrame())
        {
            PerformRaycast(instantInteraction: true);
        }
        else if (inputManager.InteractIsHeld())
        {
            PerformRaycast(instantInteraction: false);
        }
        else if (inputManager.InteractReleased())
        {
            ResetInteractionState();
        }
    }

    private void PerformRaycast(bool instantInteraction)
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hitInfo, interactDistance, layerMask))
        {
            int objectLayer = hitInfo.collider.gameObject.layer;

            if (instantInteraction && objectLayer != delayedInteractionLayer && Time.time - interactTimer >= interactDelay)
            {
                TriggerInteraction(hitInfo);
            }
            else if (!instantInteraction && objectLayer == delayedInteractionLayer)
            {
                HandleDelayedInteraction(hitInfo);
            }
            else
            {
                ResetDelayedInteractionState();
            }
        }
        else
        {
            ResetDelayedInteractionState();
        }
    }

    private void TriggerInteraction(RaycastHit hitInfo)
    {
        IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>() ?? hitInfo.collider.transform.parent.GetComponent<IInteractable>();
        interactable?.Interact(hitInfo, NetworkObject, -1);
    }

    private void HandleDelayedInteraction(RaycastHit hitInfo)
    {
        if (!isInteractingWithDelayedObject)
        {
            interactTimer = Time.time;
            isInteractingWithDelayedObject = true;
        }

        float progress = (Time.time - interactTimer) / interactDelay;

        if (progress >= delayedInteractionTime && !hasInteractedDelayed)
        {
            TriggerInteraction(hitInfo);
            hasInteractedDelayed = true;
            ResetDelayedInteractionState();
        }
        else if (hasInteractedDelayed && progress < delayedInteractionTime)
        {
            hasInteractedDelayed = false;
        }
    }

    private void ResetDelayedInteractionState()
    {
        isInteractingWithDelayedObject = false;
    }

    private void ResetInteractionState()
    {
        isInteractingWithDelayedObject = false;
        hasInteractedDelayed = false;
    }
}
