using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MMO
{
    /// <summary> Outline an object if a cursor is over the object </summary>
    public class OutlineObject : MonoBehaviour {

        // the outline of the object
        private Outline outline;
        // radius that the outline should have
        public float radius;

        /// <summary> Disable the outline at the start </summary>
        void Start() {
            outline = gameObject.GetComponent<Outline>();
            outline.enabled = false;
        }

        /// <summary> Is the player close enough for it to be outlined </summary>
        /// <returns> whether the player is close enough for the gameobject to be outlined </returns>
        private bool IsPlayerCloseEnough()
        {
            float playerDist = Vector3.Distance(NetworkClient.connection.identity.transform.position, gameObject.transform.position);
            return (playerDist < radius);
        }

        /// <summary> When the mouse hovers over a gameobject, outline the object </summary>
        void OnMouseOver() {
            if (IsPlayerCloseEnough()) {
                outline.enabled = true;
            } else {
                outline.enabled = false;
            }
        }

        /// <summary> Disable the outline when the mouse exits the gameobject </summary>
        void OnMouseExit() {
            outline.enabled = false;
        }
    }
}
