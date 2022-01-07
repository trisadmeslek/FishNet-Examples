using FishNet.Object;
using UnityEngine;

namespace FishNet.Tutorial.Ownership
{
    public class Ownership : NetworkBehaviour
    {
        /* Ownership is of extreme importance with networking. It is vital
         * to understand how ownership works.
         * 
         * Only clients may have ownership of an object, making them the owner. While the server ultimately
         * decides what happens with an object, the server itself never actually owns the object, only
         * clients may own objects. It's important to remember that when you perform base.IsOwner checks
         * you are performing them from the client, not the server.
         * 
         * Ownership allows a player to send ServerRpcs to the server, which is a common way
         * for a player to talk to the server. ServerRpcs are often used to request action by
         * the server. My Remote Calls video will cover ServerRpcs in more detail.
         * 
         * Generally ownership represents which player has the power to modify an object, such as
         * your character, or an item being held. But the uses are limitless.
         * 
         * The PlayerSpawner demo script illustrates how to spawn an object for a client, giving them
         * ownership, when they join the server.
         * 
         * There are also several other ways to receive ownership; below are some common options. */

        /// <summary>
        /// Prefab to respawn with.
        /// </summary>
        private GameObject _somePrefab;

        private void Update()
        {
            bool playerDead = false;
            /* I'm using base.IsOwner to check for ownership before calling
             * RequestRespawn(). */
            if (playerDead && base.IsOwner)
                ServerRequestRespawn();

            RequestOwnershipOnClick();
        }


        [ServerRpc]
        private void ServerRequestRespawn()
        {
            GameObject result = Instantiate(_somePrefab);
            //Instantiate over the server giving ownership to the client which owns this script.
            base.Spawn(result, base.Owner);
        }


        /// <summary>
        /// Traces for an object in the scene then tries to take ownership of it.
        /// </summary>
        private void RequestOwnershipOnClick()
        {
            /* This logic ultimately sends a serverRpc. Since server rpcs cannot be
             * sent without ownership there is no reason to continue if the
             * client does not have ownership. */
            if (!base.IsOwner)
                return;
            //Mouse not pressed, exit method.
            if (!Input.GetKeyDown(KeyCode.Mouse0))
                return;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                NetworkObject nob = hit.collider.GetComponent<NetworkObject>();
                /* If other object has a NetworkObject and client doesn't already
                 * own that object then request ownership for it. */
                if (nob != null && !nob.IsOwner)
                {
                    Debug.Log("Sending request ownership for " + hit.collider.gameObject.name);
                    ServerRequestOwnership(nob);
                }
            }
        }


        [ServerRpc]
        private void ServerRequestOwnership(NetworkObject nob)
        {
            Debug.Log("Received request ownership for " + nob.gameObject.name);
            /* Let's assume this method is being run on PlayerA while
             * nob belongs to PlayerB. We are telling the
             * NetworkObject on PlayerB to assign ownership to
             * the client that owns the object this
             * script runs on, which is PlayerA. */
            nob.GiveOwnership(base.Owner);
        }


    }


}