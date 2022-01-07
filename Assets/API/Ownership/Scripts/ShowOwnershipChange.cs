using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace FishNet.Tutorial.Ownership
{
    public class ShowOwnershipChange : NetworkBehaviour
    {

        /// <summary>
        /// Called on the client after ownership has changed.
        /// </summary>
        /// <param name="prevOwner">Previous owner of this object.</param>
        public override void OnOwnershipClient(NetworkConnection prevOwner)
        {
            base.OnOwnershipClient(prevOwner);
            if (base.IsOwner)
                GetComponent<SpriteRenderer>().color = Color.blue;
        }

        /// <summary>
        /// Called on the server after ownership has changed.
        /// </summary>
        /// <param name="prevOwner">Previous owner of this object.</param>
        public override void OnOwnershipServer(NetworkConnection prevOwner)
        {
            base.OnOwnershipServer(prevOwner);
            Debug.Log("Ownership has changed on the server.");
        }
    }

}