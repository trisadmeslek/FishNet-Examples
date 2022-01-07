using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using UnityEngine;

namespace FishNet.Tutorial.RemoteCalling
{
    public class RemoteCalls : NetworkBehaviour
    {
        public enum RemoteCallType
        {
            Disabled,
            ServerRpc,
            ObserversRpc,
            TargetRpc
        }

        /// <summary>
        /// Type of remote call to begin on start.
        /// </summary>
        [SerializeField]
        private RemoteCallType _remoteCallType = RemoteCallType.ServerRpc;

        private void Awake()
        {
            StartCoroutine(__ChangeColor());
        }

        private IEnumerator __ChangeColor()
        {
            WaitForSeconds wait = new WaitForSeconds(3f);
            while (true)
            {
                //ServerRpc.
                if (_remoteCallType == RemoteCallType.ServerRpc && base.IsOwner)
                    ServerSetColor(RandomColor());
                //ObserversRpc.
                else if (_remoteCallType == RemoteCallType.ObserversRpc && base.IsServer)
                    ObserversSetColor(RandomColor());
                else if (_remoteCallType == RemoteCallType.TargetRpc && base.IsServer && base.OwnerIsActive)
                    TargetSetColor(base.Owner, RandomColor());

                //Wait a short duration.
                yield return wait;

            }
        }


        [ServerRpc(RequireOwnership = true)]
        private void ServerSetColor(Color c)
        {
            Debug.Log("Got ServerRpc.");
            SetColor(c);
        }

        [ObserversRpc(BufferLast = true, IncludeOwner = true)]
        private void ObserversSetColor(Color c)
        {
            Debug.Log($"Got ObserversRpc on connectionId {base.LocalConnection.ClientId}.");
            SetColor(c);
        }

        [TargetRpc]
        private void TargetSetColor(NetworkConnection conn, Color c)
        {
            Debug.Log($"Got TargetRpc on connectionId {conn.ClientId}.");
            SetColor(c);
        }

        private void SetColor(Color c)
        {
            GetComponent<SpriteRenderer>().color = c;
        }

        private Color RandomColor()
        {
            Color c = Random.ColorHSV();
            c.a = 1f;
            return c;
        }

    }


}