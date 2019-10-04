using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
    
    private Outline outline;

    void Start() {
        outline = gameObject.GetComponent<Outline>();
        outline.enabled = false;
    }
    public void Interact() {
        gameObject.transform.Rotate(0, 90, 0);
    }

    void OnMouseEnter() {
        outline.enabled = true;
    }
    void OnMouseExit() {
        outline.enabled = false;
    }
}
