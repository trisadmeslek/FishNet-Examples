using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using System.Collections;
using UnityEngine;

namespace FishNet.Tutorial.SyncVaring
{
    public class ColorChanger : NetworkBehaviour
    {
        [SyncVar(Channel = Channel.Unreliable, ReadPermissions = ReadPermission.Observers, SendRate = 0.1f, OnChange = nameof(OnColor))]
        private Color _color;

        /// <summary>
        /// Called when the _color field changes.
        /// </summary>
        /// <param name="prev">Value before change.</param>
        /// <param name="next">Value after change.</param>
        /// <param name="asServer">True if callback is occuring on server, false if on client.</param>
        private void OnColor(Color prev, Color next, bool asServer)
        {
            if (!asServer)
                GetComponent<SpriteRenderer>().color = next;
            else
                Debug.Log($"New color: {next}.");
        }

        /// <summary>
        /// Called on the server after initializing this object.
        /// SyncTypes modified before or during this method will be sent to clients in the spawn message.
        /// </summary> 
        public override void OnStartServer()
        {
            base.OnStartServer();
            StartCoroutine(__ChangeColor());
        }

        private IEnumerator __ChangeColor()
        {
            WaitForSeconds wait = new WaitForSeconds(1f);
            while (true)
            {
                //Wait a short duration.
                yield return wait;
                //Change syncVar.
                Color c = Random.ColorHSV();
                c.a = 1f;
                _color = c;
            }
        }


    }


}