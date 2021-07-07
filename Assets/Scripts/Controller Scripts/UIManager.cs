using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Canvas pauseMenu;
    public Canvas dialogueBox;
    public GameObject inventory;
    public Text dialogueText;

    private AudioSource audioSource;
    private Dialogue[] dialogue;
    private int currentDialogueIndex = 0;

    public bool togglePauseMenu;
    public bool toggleDialogueBox;
    public bool canMove = true;

    void Start()
    {
        dialogueText = dialogueText.GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();
        pauseMenu.enabled = false;
        dialogueBox.enabled = false;
    }

    public void ToggleDialogue(Dialogue[] dialogue)
    {
        canMove = toggleDialogueBox;
        currentDialogueIndex = -1;
        toggleDialogueBox = !toggleDialogueBox;
        dialogueBox.enabled = !dialogueBox.enabled;
        this.dialogue = dialogue;

        if (dialogueBox.enabled) PlayDialogue();
    }

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

    public void TogglePauseMenu()
    {
        canMove = !canMove;
        togglePauseMenu = !togglePauseMenu;
        pauseMenu.enabled = !pauseMenu.enabled;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
