using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// outline the opject
public class OutlineObject : MonoBehaviour {

    // outline
    private Outline outline;
    //radius in order for outline 
    public float radius;
    // disable outline at start
    void Start() {
        outline = gameObject.GetComponent<Outline>();
        outline.enabled = false;
    }

    // is the player close enough for it to be outlined
    private bool PlayerCloseEnough()
    {
        float playerDist = Vector3.Distance(NetworkClient.connection.identity.transform.position, gameObject.transform.position);
        return (playerDist < radius);
    }

    // enable outline at appropriate times
    void OnMouseOver() {
        if (PlayerCloseEnough()) {
            outline.enabled = true;
        } else {
            outline.enabled = false;
        }
    }

    // diable when mouse exits
    void OnMouseExit() {
        outline.enabled = false;
    }
}
