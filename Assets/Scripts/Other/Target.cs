using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MMO
{
    public class Target : MonoBehaviour
    {
        public virtual IEnumerator Interact() {
            Debug.Log("Override this");
            yield break;
        }
    }
}
