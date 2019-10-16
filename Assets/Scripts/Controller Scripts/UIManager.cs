using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Canvas pauseMenu;
    public Canvas dialogueBox;
    public Canvas inventory;
    public Text dialogueText;
    private int boxes = 0;
    private bool talking = false;
    private bool firstLine = false;
    private string[] dialogue;
    private AudioClip[] sounds;
    private AudioSource audioData;
    private bool soundPlaying;

    public bool togglePauseMenu;
    public bool toggleDialogueBox;
    public bool canMove = true;

    void Start()
    {
        dialogueText = dialogueText.GetComponent<Text>();
        audioData = GetComponent<AudioSource>();
        pauseMenu.enabled = false;
        dialogueBox.enabled = false;
    }

    void Update()
    {
        if (talking && (boxes >= 0))
        {
            if (!firstLine)
            {
                Debug.Log("First line, starting with " + boxes + " boxes");
                dialogueText.text = dialogue[dialogue.Length - boxes];
                audioData.Stop();
                audioData.clip = sounds[dialogue.Length - boxes];
                audioData.Play();
                firstLine = true;
                boxes--;
                Debug.Log("boxes after: " + boxes);
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    if (boxes != 0)
                    {
                        Debug.Log("Hit return, boxes remaining: " + boxes + " boxes");
                        audioData.Stop();
                        dialogueText.text = dialogue[dialogue.Length - boxes];
                        audioData.clip = sounds[dialogue.Length - boxes];
                        audioData.Play();
                        boxes--;
                        Debug.Log("boxes after: " + boxes);
                    }
                    else
                    {
                        audioData.Stop();
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

    public void TogglePauseMenu()
    {
        canMove = !canMove;
        togglePauseMenu = !togglePauseMenu;
        pauseMenu.enabled = !pauseMenu.enabled;
    }

    public void ToggleDialogueBox(string[] d, AudioClip[] a)
    {
        canMove = false;
        toggleDialogueBox = !toggleDialogueBox;
        dialogueBox.enabled = !dialogueBox.enabled;
        dialogue = d;
        boxes = d.Length;
        sounds = a;
        talking = true;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
