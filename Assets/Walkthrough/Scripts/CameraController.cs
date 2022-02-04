using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private void Awake()
    {
        FirstObjectNotifier.OnFirstObjectSpawned += FirstObjectNotifier_OnFirstObjectSpawned;
    }

    private void OnDestroy()
    {
        FirstObjectNotifier.OnFirstObjectSpawned -= FirstObjectNotifier_OnFirstObjectSpawned;
    }

    /// <summary>
    /// Called when the local clients first object spawns.
    /// </summary>
    private void FirstObjectNotifier_OnFirstObjectSpawned(Transform obj)
    {
        CinemachineVirtualCamera vc = GetComponent<CinemachineVirtualCamera>();
        vc.Follow = obj;
        vc.LookAt = obj;
    }


}
