using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Core
{
    public interface IInteractable
    {
        public void Interact(ReferenceHub interactor, RaycastHit hit, out bool interactSuccessful);
        public UnityAction<IInteractable> OnInteractionComplete {get; set;}
    }   

}
