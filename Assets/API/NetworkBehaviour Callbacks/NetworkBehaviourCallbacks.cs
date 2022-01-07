using FishNet.Connection;
using FishNet.Object;

namespace FishNet.Tutorial.NetworkBehaviourCallbacks
{

    public class NetworkBehaviourCallbacks : NetworkBehaviour
    {

        /// <summary>
        /// Called when the network has initialized this object. May be called for server or client but will only be called once.
        /// 
        /// Great for initializations that must be done on server and client, such as subscribing to events.
        /// When as host or server this method will run before OnStartServer. When as client only the method will run before OnStartClient.
        /// </summary>
        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
        }
        /// <summary>
        /// Called on the server after initializing this object.
        /// SyncTypes modified before or during this method will be sent to clients in the spawn message.
        /// 
        /// Can be used to initialize server side logic or update SyncTypes so they are included within the spawn message.
        /// SyncTypes modified here will arrive on the client before OnStartClient, but if you want to send a RPC use OnSpawnServer.
        /// If a SyncType has a callback, such as OnChange, that callback will occur after OnStartServer.
        /// </summary> 
        public override void OnStartServer()
        {
            base.OnStartServer();
        }
        /// <summary>
        /// Called on the server after ownership has changed.
        /// </summary>
        /// <param name="prevOwner">Previous owner of this object.</param>
        public override void OnOwnershipServer(NetworkConnection prevOwner)
        {
            base.OnOwnershipServer(prevOwner);
        }
        /// <summary>
        /// Called on the server after a spawn message for this object has been sent to clients.
        /// 
        /// Useful for sending remote calls or data to clients. Any data sent here will arrive after the spawn message, and after OnStartClient.
        /// This method will call for every connection the object spawns for.
        /// </summary>
        /// <param name="connection">Connection the object is being spawned for.</param>
        public override void OnSpawnServer(NetworkConnection connection) 
        {
            base.OnSpawnServer(connection);
        }
        /// <summary>
        /// Called on the server before a despawn message for this object has been sent to connection.
        /// 
        /// Useful for sending remote calls or actions to clients. Any data sent here will arrive before the despawn message, and before OnStopClient.
        /// </summary>
        public override void OnDespawnServer(NetworkConnection connection) 
        {
            base.OnDespawnServer(connection);
        }
        /// <summary>
        /// Called on the server before deinitializing this object.
        /// 
        /// Can be used to deinitialize the object such as unsubscribe from events.
        /// </summary>
        public override void OnStopServer()
        {
            base.OnStopServer();
        }
        /// <summary>
        /// Called on the client after initializing this object.
        /// 
        /// By the time this method is called all network information and SyncTypes are synchronized with the client.
        /// You may initialize for the owning client by checking 'if (base.IsOwner)' within this method.
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();
        }
        /// <summary>
        /// Called on the client before deinitializing this object.
        /// </summary>
        public override void OnStopClient()
        {
            base.OnStopClient();
        }
        /// <summary>
        /// Called on the client after ownership has changed.
        /// 
        /// Can be used to perform actions if losing or gaining ownership.
        /// Useful methods might be 'base.IsOwner', or 'prevOwner.IsLocalClient'.
        /// </summary>
        /// <param name="prevOwner">Previous owner of this object.</param>
        public override void OnOwnershipClient(NetworkConnection prevOwner) 
        {
            base.OnOwnershipClient(prevOwner);
        }
        /// <summary>
        /// Called when the network is deinitializing this object. May be called for server or client but will only be called once.
        /// 
        /// Like OnStartNetwork this will only call once, even if running as both server and client.
        /// If you are running as host or server this method will run after OnStopServer. When as client only this
        /// method will run after OnStopClient.
        /// </summary>
        public override void OnStopNetwork()
        {
            base.OnStopNetwork();
        }

        /* Order of server callbacks.
         * 
         * OnStartNetwork
         * 
         * OnStartServer
         * OnOwnershipServer (when an owner is added, changed, or removed)
         * OnSpawnServer (when being spawned for a client for any reason)
         * OnDespawnServer (when being despawned for a client for any reason)
         * OnStopServer 
         * 
         * OnStopNetwork
         */

        /* Order of client callbacks.
         * 
         * OnStartNetwork (if client only)
         * 
         * OnStartClient
         * OnOwnershipClient (when an owner is added, changed, or removed)
         * OnStopClient
         * 
         * OnStopNetwork (if client only)
         */

        /* Some callbacks may occur as the triggering action happens.
         * 
         * OnOwnershipServer
         * OnSpawnServer
         * OnDespawnServer
         * OnOwnershipClient
         */

        /* Very important. When running as host a tick may pass between
         * the server callbacks and the client callbacks. This is intentional as
         * the server provides the callbacks first, then the client provides them
         * after receiving the information over the network. */
    }


}