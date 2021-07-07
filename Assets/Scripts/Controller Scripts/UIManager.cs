using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // pause menu canvas
    public Canvas pauseMenu;
    // dialogue box canvax
    public Canvas dialogueBox;
    // dialogue text box
    public Text dialogueText;
    public Canvas compassCanvas, mapCanvas;

    // source to play audio from
    private AudioSource audioSource;
    // dialogue strings
    private Dialogue[] dialogue;
    // current place in dialogue
    private int currentDialogueIndex = 0;
    private Map map;

    public bool togglePauseMenu = false, toggleDialogueBox = false, toggleMap = false;
    // if player can move
    public static bool canMove = true;

    // instantiate variables
    void Start()
    {
        dialogueText = dialogueText.GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
        map = mapCanvas.GetComponent<Map>();
        pauseMenu.enabled = togglePauseMenu;
        dialogueBox.enabled = toggleDialogueBox;
        mapCanvas.enabled = toggleMap;
        compassCanvas.enabled = !toggleMap;
    }

    // turn dialogue box on and off
    public void ToggleDialogue(Dialogue[] dialogue)
    {
        canMove = toggleDialogueBox;
        currentDialogueIndex = -1;
        toggleDialogueBox = !toggleDialogueBox;
        dialogueBox.enabled = !dialogueBox.enabled;
        this.dialogue = dialogue;

        if (dialogueBox.enabled) PlayDialogue();
    }

    // play a new line of audio
    public void PlayDialogue()
    {
        Debug.Log("PLAY");

        currentDialogueIndex++;
        audioSource.Stop();

        if (currentDialogueIndex > dialogue.Length - 1)
        {
            ToggleDialogue(dialogue);
            return;
        }

        dialogueText.text = dialogue[currentDialogueIndex].text;
        audioSource.PlayOneShot(dialogue[currentDialogueIndex].audio);
    }

    // turn on / off the pause menu
    public void TogglePauseMenu()
    {
        canMove = !canMove;
        togglePauseMenu = !togglePauseMenu;
        pauseMenu.enabled = !pauseMenu.enabled;
    }

    public void ToggleMap()
    {
        toggleMap = !toggleMap;
        if (toggleMap) map.Enable();
        else map.Disable();
        mapCanvas.enabled = toggleMap;
        compassCanvas.enabled = !toggleMap;

    }

    // lock the cursor
    public static void LockCursor(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}



