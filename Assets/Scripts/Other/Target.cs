using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMO
{
    public class Target : MonoBehaviour
    {
        public virtual void Interact() {
            Debug.Log("Override this");
        }
    }
}
