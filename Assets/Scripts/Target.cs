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
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 lookVector = player.transform.position - transform.position;
        //lookVector.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(lookVector, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1);
    }

    void OnMouseEnter() {
        outline.enabled = true;
    }
    void OnMouseExit() {
        outline.enabled = false;
    }
}
