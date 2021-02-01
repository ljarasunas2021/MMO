using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public virtual void Interact() {
        Debug.Log("Override this");
    }
}
