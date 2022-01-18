using FishNet.Object;
using System;
using UnityEngine;

public class FirstObjectNotifier : NetworkBehaviour
{
    /// <summary>
    /// Called when the local clients first object spawns.
    /// </summary>
    public static event Action<Transform> OnFirstObjectSpawned;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            NetworkObject nob = base.LocalConnection.FirstObject;
            if (nob == base.NetworkObject)
                OnFirstObjectSpawned?.Invoke(transform);
        }
    }
}
