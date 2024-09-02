using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations;

public sealed class ReferenceHub : NetworkBehaviour, IEquatable<ReferenceHub>
{
    public PlayerInventoryHolder inventory;
    public NetworkObject networkObject;
    public PlayerMovement playerMovement;
    public Interaction interaction;
    public ClientNetworkTransform clientNetworkTransform;
    public InputManager inputManager;
	public ParentConstraint itemConstraint;

	public ulong PlayerId => NetworkObject.OwnerClientId;
    public static Action<ReferenceHub> OnPlayerAdded;
	public static Action<ReferenceHub> OnPlayerRemoved;
	public static HashSet<ReferenceHub> AllHubs { get; private set; } = new HashSet<ReferenceHub>();
	private static readonly Dictionary<GameObject, ReferenceHub> HubsByGameObjects = new(20, new GameObjectComparer());
	private static readonly Dictionary<ulong, ReferenceHub> HubByPlayerIds = new(20);


    void Awake()
    {
        AllHubs.Add(this);
        HubByPlayerIds.Add(OwnerClientId, this);
        HubsByGameObjects[gameObject] = this;
        SetupComponents();
    }

    void Start()
    {
		OnPlayerAdded?.Invoke(this);

    }

    public override void OnDestroy()
    {
		AllHubs.Remove(this);
		HubsByGameObjects.Remove(gameObject);
    }
	public static bool TryGetHub(ulong playerId, out ReferenceHub hub)
	{
		if (playerId > 0)
		{
			if (HubByPlayerIds.TryGetValue(playerId, out hub))
				return true;

			foreach (ReferenceHub reference in AllHubs)
			{
				if (reference.PlayerId != playerId)
					continue;

				HubByPlayerIds[playerId] = reference;
				hub = reference;
				return true;
			}
		}

		hub = null;
		return false;
	}

    private void SetupComponents()
    {
        if (networkObject == null)
            networkObject = GetComponent<NetworkObject>();
        if (inventory == null)
            inventory = GetComponent<PlayerInventoryHolder>();
        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();
        if (interaction == null)
            interaction = GetComponent<Interaction>();
        if (clientNetworkTransform == null)
            clientNetworkTransform = GetComponent<ClientNetworkTransform>();
        if (inputManager == null)
            inputManager = GetComponent<InputManager>();
    }

    private class GameObjectComparer : EqualityComparer<GameObject>
	{
		public override bool Equals(GameObject x, GameObject y)
		{
			return x == y;
		}

		public override int GetHashCode(GameObject obj)
		{
			return obj == null ? 0 : obj.GetHashCode();
		}
	}

	#region IEquatable
	public bool Equals(ReferenceHub other)
	{
		return this == other;
	}

	public override bool Equals(object obj)
	{
		return obj is ReferenceHub other && this == other;
	}

	public override int GetHashCode()
	{
		return gameObject.GetHashCode();
	}

	public static bool operator ==(ReferenceHub left, ReferenceHub right)
	{
		return (MonoBehaviour) left == right;
	}

	public static bool operator !=(ReferenceHub left, ReferenceHub right)
	{
		return (MonoBehaviour) left != right;
	}
	#endregion
}
