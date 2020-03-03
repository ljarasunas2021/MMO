using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class OutlineObject : MonoBehaviour {

    private Outline outline;
    public float radius;
    void Start() {
        outline = gameObject.GetComponent<Outline>();
        outline.enabled = false;
    }

    private bool PlayerCloseEnough()
    {
        float playerDist = Vector3.Distance(NetworkClient.connection.identity.transform.position, gameObject.transform.position);
        return (playerDist < radius);
    }

    void OnMouseOver() {
        if (PlayerCloseEnough()) {
            outline.enabled = true;
        } else {
            outline.enabled = false;
        }
    }
    void OnMouseExit() {
        outline.enabled = false;
    }
}
