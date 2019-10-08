using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Target : MonoBehaviour {
    
    private Outline outline;
    private UIManager UIScript;
    public string[] dialogue;
    public AudioClip[] inputSounds;

    void Start() {
        UIScript = GameObject.Find("UI Manager").GetComponent<UIManager>();
        outline = gameObject.GetComponent<Outline>();
        outline.enabled = false;
    }
    public void Interact() {
        if (UIScript.canMove) {
            transform.LookAt(NetworkClient.connection.identity.transform);

            UIScript.ToggleDialogueBox(dialogue, inputSounds);
        }
    }

    void OnMouseEnter() {
        outline.enabled = true;
    }
    void OnMouseExit() {
        outline.enabled = false;
    }
}
