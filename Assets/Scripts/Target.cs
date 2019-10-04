using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {

    public void Interact() {
        gameObject.transform.Rotate(0, 90, 0);
    }
}
