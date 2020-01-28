using UnityEngine;
using System.Collections;

namespace AnimFollow
{
    public class Limb_AF : MonoBehaviour
    {
        public readonly int version = 7; // The version of this script

        // This script is distributed (automatically by RagdollControl) to all rigidbodies and reports to the RagdollControl script if any limb is currently colliding.

        RagdollControl_AF ragdollControl;
        string[] ignoreCollidersWithTag;

        void OnEnable()
        {
            ragdollControl = transform.root.GetComponentInChildren<RagdollControl_AF>();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!(collision.transform.name == "Terrain") && collision.transform.root != this.transform.root)
            {
                ragdollControl.numberOfCollisions++;
                float collisionSpeed = collision.relativeVelocity.magnitude;
                if (collisionSpeed > ragdollControl.collisionSpeed) ragdollControl.collisionSpeed = collisionSpeed;
            }
        }

        void OnCollisionExit(Collision collision)
        {
            bool ignore = false;
            if (!(collision.transform.name == "Terrain") && collision.transform.root != this.transform.root)
            {
                ragdollControl.numberOfCollisions--;
            }
        }
    }
}
