using FishNet.Object;
using FishNet.Object.Prediction;
using UnityEngine;

namespace FishNet.Tutorial.Prediction
{

    public class PredictionMotor : NetworkBehaviour
    {
        #region Types.
        /* It's strongly recommended to use structures for your data.
         * Datas are cached on client and server, and will create garbage
         * if you use a class. */

        /* MoveData may be named whatever you like. In my script it's used to
         * store client inputs, which are later used to move the object identically
         * on the server and owner. */
        private struct MoveData
        {
            public float Horizontal;
            public float Vertical;
            public MoveData(float horizontal, float vertical)
            {
                Horizontal = horizontal;
                Vertical = vertical;
            }
        }
        
        /* ReconcileData may also be named differently. This contains data about how
         * to reset the object to the server values. These values will be sent to the client. */
        private struct ReconcileData
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Velocity;
            public Vector3 AngularVelocity;
            public ReconcileData(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
            {
                Position = position;
                Rotation = rotation;
                Velocity = velocity;
                AngularVelocity = angularVelocity;
            }
        }
        #endregion

        #region Misc
        /// <summary>
        /// How much force to add per move.
        /// </summary>
        public float MoveRate = 30f;
        /// <summary>
        /// Rigidbody on this object.
        /// </summary>
        private Rigidbody _rigidbody;
        /// <summary>
        /// True if subscribed to the TimeManager.
        /// </summary>
        private bool _subscribed = false;
        #endregion

        private void Awake()
        {
            /* Both the server and owner must have a reference to the rigidbody.
             * Forces are applied to both the owner and server so that the objects
             * move the same. This value could be set in OnStartServer and OnStartClient
             * but to keep it simple I'm using Awake. */
            _rigidbody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Subscribe or unsubscribe to the TimeManager for Tick events.
        /// </summary>
        /// <param name="subscribe"></param>
        private void SubscribeToTimeManager(bool subscribe)
        {
            //TimeManager could be null if exiting the application or not yet initialized.
            if (base.TimeManager == null)
                return;

            /* If already subscribed/unsubscribed there
             * is no need to do it again. */
            if (subscribe == _subscribed)
                return;
            _subscribed = subscribe;

            if (subscribe)
            {
                base.TimeManager.OnTick += TimeManager_OnTick;
                base.TimeManager.OnPostTick += TimeManager_OnPostTick;
            }
            else
            {
                base.TimeManager.OnTick -= TimeManager_OnTick;
                base.TimeManager.OnPostTick -= TimeManager_OnPostTick;
            }
        }

        private void OnDestroy()
        {
            /* When the object is destroyed remove its TimeManager
             * subscription. This is so the events are not calling
             * to a null object. */
            SubscribeToTimeManager(false);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            /* The TimeManager won't be set until at least
             * OnStartClient or OnStartServer, so do not
             * try to subscribe before these events. */
            SubscribeToTimeManager(true);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            /* The TimeManager won't be set until at least
            * OnStartClient or OnStartServer, so do not
            * try to subscribe before these events. */
            SubscribeToTimeManager(true);
        }

        /* OnTick is the equivalent to FixedUpdate. OnTick is called
         * right before physics are updated. */
        private void TimeManager_OnTick()
        {
            if (base.IsOwner)
            {
                /* Reconciliation must be done first.
                 * This will correct the clients position to as it is
                 * on the server and replay cached client inputs.
                 * When using reconcile on the client default
                 * should be passed in as the data, and false for asServer.
                 * This indicates a client-side reconcile. */
                Reconciliation(default, false);
                /* Gather data needed to know how the object is moved. This is used
                 * by the server and client. */
                MoveData data;
                GatherInputs(out data);
                /* When moving on the client pass in the just gathered data, and
                 * false for asServer. This will move the client locally using data.
                 * You do not need to pass in the replaying value. */
                Move(data, false);
            }

            if (base.IsServer)
            {
                /* Server has to move the same as client; this helps keep the object in sync.
                 * Pass in default for the data, and true for asServer. The server automatically
                 * knows which data to use when asServer is true. Like when calling from client,
                 * you do not need to set replaying. */
                Move(default, true);

                /* As shown below the reconcile is sent using OnPostTick because you will
                 * want to send the objects position, rotation, ect, after the physics have simulated.
                 * If you are using a method to move your object that does not rely on physics, such as
                 * a character controller or moving the transform directly, you may opt-out of using
                 * OnPostTick and send the Reconcile here. */
            }
        }

        /* OnPostTick is after physics have simulated for the tick. */
        private void TimeManager_OnPostTick()
        {
            /* Build the reconcile using current data of the object. This is sent to the client, and the
             * client will reset using these values. It's EXTREMELY important to send anything that might
             * affect the movement, position, and rotation of the object. This includes but is not limited to: 
             * transform position and rotation, rigidbody velocities, colliders, ect. 
             * 
             * Explained further: if you are using prediction on a vehicle that is controlled by wheel colliders, those
             * colliders most likely will behave independently of the vehicle root. You must send the colliders position,
             * rotation, and any other value that can change from movement or affect movement.
             * 
             * Another example would be running with stamina. If running depends on stamina you will want to also
             * send back stamina along with running state so that the client can adjust their side locally if it differs.
             * If stamina somehow existed on the client but not the server then the server would move slower and a desync
             * would occur. If you did not send stamina/run state back the client would continue to desync until they also
             * ran out of stamina.
             * 
             * If you are using an asset that uses physics internally there is a fair chance you will need to expose values
             * that affect movement or ask the author to make the asset support prediction. */

            /* When all data is reset properly the chances of a desync are very low, and near impossible when not using physics.
             * Even when a desync does occur it's often incredibly small and will be corrected without any visual disturbances.
             * There are some cases however where if a desync is serious enough the client may teleport to the corrected value.
             * I've included a component to help reduce any visual jitter during large desyncs. */
            ReconcileData data = new ReconcileData(
                transform.position, transform.rotation, _rigidbody.velocity, _rigidbody.angularVelocity
                );
            /* After building the data to send back to the client pass it into the reconcile method,
             * while using true for asServer. You should call the reconcile method every tick on both
             * the server and client. Fish-Networking internally knows if there is new data to send or not
             * and will not waste bandwidth by regularly resending unchanged data. */
            Reconciliation(data, true);
        }


        /* GatherInputs takes local inputs of the client and puts them into MoveData.
         * When no inputs are available the method is exited early. Note that data is set
         * to default at the very beginning of the method. You should pass default data into the
         * replicate method. Like when the server sends reconcile, the data will send redundantly to help
         * ensure it goes through, and also will stop sending data that hasn't changed after a few iterations.
         * You are welcome to always fill out data instead of sending default when there is no input
         * but this will cost you bandwidth. */
        private void GatherInputs(out MoveData data)
        {
            //Set to default.
            data = default;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            if (horizontal == 0f && vertical == 0f)
                return;

            //If there is input then populate data.
            data = new MoveData(horizontal, vertical);
        }


        [Replicate]
        private void Move(MoveData data, bool asServer, bool replaying = false)
        {
            /* You can use asServer to know if the server is calling this method
             * or the client. */

            /* Replaying may be true when as client and when inputs are being replayed.
             * When you call Move replaying is false, as you are
             * manually calling the method. However, when the client reconciles, cached
             * inputs are replayed automatically. This is in part how prediction works.
             * A good example of how you might use the replaying boolean is to show
             * a special effect when jumping.
             * 
             * When replaying is false you are calling the method from your code, and perhaps
             * if input indicates the player is jumping you will want to play audio or a special
             * effect. However, when cached inputs are automatically replayed the same jump
             * input may be called multiple times, but replaying will be true. You can filter
             * out playing the audio/vfx multiple times by not running the logic if replaying
             * is true. */
            Vector3 force = new Vector3(data.Horizontal, 0f, data.Vertical) * MoveRate;
            _rigidbody.AddForce(force);
        }

        /* Reconcile is responsible for resetting the clients object using data from
         * the server. You must specify what to reset but Fish-Networking will automatically
         * replay cached data, apply physics per replay, and so on.
         * 
         * With that said, physic simulations are performed with every data replayed. If you have other
         * physics objects in the same physics scenes they will also simulate when this object
         * is replaying datas. You resolve this behavior by putting this object or other objects
         * in their own physics scene. In addition, there are a varity of events and components that can be used
         * to reset other objects during a reconcile as well. These are covered in another video. You may find
         * a link in the description of this video. */
        [Reconcile]
        private void Reconciliation(ReconcileData data, bool asServer)
        {
            transform.position = data.Position;
            transform.rotation = data.Rotation;
            _rigidbody.velocity = data.Velocity;
            _rigidbody.angularVelocity = data.AngularVelocity;
        }

    }

}