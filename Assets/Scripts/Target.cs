using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Target : MonoBehaviour {
    
    private Outline outline;
    private UIManager UIScript;
    public string[] dialogue;
    public AudioClip[] inputSounds;

    public string interactKey;

    private float radius = 6f;

    void Start() {
        UIScript = GameObject.Find("UI Manager").GetComponent<UIManager>();
        outline = gameObject.GetComponent<Outline>();
        outline.enabled = false;
    }
    public void Interact() {
        if (PlayerCloseEnough()) {
            switch (interactKey){
                case "npc":
                    if (UIScript.canMove) {
                    transform.LookAt(NetworkClient.connection.identity.transform);

                    UIScript.ToggleDialogueBox(dialogue, inputSounds);
                    }
                    break;
                case "device":
                    break;
            }
        }
    }

    void OnMouseOver() {
        if (PlayerCloseEnough()) {
            if (UIScript.canMove && !outline.enabled) {
                outline.enabled = true;
            }
        } else {
            outline.enabled = false;
        }
    }
    void OnMouseExit() {
        outline.enabled = false;
    }

    private bool PlayerCloseEnough() {
        float playerDist = Vector3.Distance(NetworkClient.connection.identity.transform.position, gameObject.transform.position);
        if (playerDist < radius) {
            return true;
        } else {
            return false;
        }
    }
}
