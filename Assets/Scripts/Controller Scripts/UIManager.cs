using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private Canvas dialogueBox;
    [SerializeField] private Text dialogueText;
    private int boxes = 0;
    private bool talking = false;
    private bool firstLine = false;
    private string[] dialogue;

    public bool togglePauseMenu;
    public bool toggleDialogueBox;
    public bool canMove = true;

    void Start() {
        dialogueText = dialogueText.GetComponent<Text>();
        pauseMenu.enabled = false;
        dialogueBox.enabled = false;
    }

    void Update() {
        // if (!isLocalPlayer) {
        //     return;
        // }
        if (talking && (boxes >= 0)) {
            if (!firstLine) {
                Debug.Log("First line, starting with " + boxes + " boxes");
                dialogueText.text = dialogue[dialogue.Length-boxes];
                firstLine = true;
                boxes--;
                Debug.Log("boxes after: " + boxes);
                // if (boxes == 0) {
                //     canMove = true;
                //     toggleDialogueBox = !toggleDialogueBox;
                //     dialogueBox.enabled = !dialogueBox.enabled;
                //     talking = false;
                //     firstLine = false;
                // }
            } else {
                if (Input.GetKeyDown(KeyCode.Return)) {
                    if (boxes != 0) {
                        Debug.Log("Hit return, boxes remaining: " + boxes + " boxes");
                        dialogueText.text = dialogue[dialogue.Length-boxes];
                        boxes--;
                        Debug.Log("boxes after: " + boxes);
                    } else {
                        canMove = true;
                        toggleDialogueBox = !toggleDialogueBox;
                        dialogueBox.enabled = !dialogueBox.enabled;
                        talking = false;
                        firstLine = false;
                    }
                }
            }
        }
    }

    public void TogglePauseMenu() {
        canMove = !canMove;
        togglePauseMenu = !togglePauseMenu;
        pauseMenu.enabled = !pauseMenu.enabled;
    }

    public void ToggleDialogueBox(string[] d) {
        canMove = false;
        toggleDialogueBox = !toggleDialogueBox;
        dialogueBox.enabled = !dialogueBox.enabled;
        dialogue = d;
        boxes = d.Length;
        talking = true;
    }

    public void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
