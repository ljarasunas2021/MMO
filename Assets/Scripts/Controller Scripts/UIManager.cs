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
    public Canvas compassCanvas, mapCanvas, dialogueCanvas;

    // source to play audio from
    [HideInInspector] public AudioSource audioSource;
    // dialogue strings
    private NPCDialogue dialogue;
    // current place in dialogue
    private Map map;

    public bool togglePauseMenu = false, toggleDialogueBox = false, toggleMap = false;
    public static bool canMove = true;

    void Start()
    {
        dialogueText = dialogueText.GetComponent<Text>();
        map = mapCanvas.GetComponent<Map>();
        pauseMenu.enabled = togglePauseMenu;
        dialogueBox.enabled = toggleDialogueBox;
        mapCanvas.enabled = toggleMap;
        compassCanvas.enabled = !toggleMap;
    }

    public void ToggleDialogue(NPCDialogue dialogue)
    {
        canMove = (dialogue == null);
        dialogueBox.enabled = (dialogue != null);
        if (!canMove) PlayDialogue(dialogue);
    }

    public void PlayDialogue(NPCDialogue dialogue)
    {
        dialogueText.text = dialogue.text;
        audioSource.PlayOneShot(dialogue.audio);
        StartCoroutine(WaitUntilAudioIsDone(dialogue));
    }

    private IEnumerator WaitUntilAudioIsDone(NPCDialogue dialogue)
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        if (dialogue.nextDialogueIsNPC && dialogue.nextDialogue[0] != null)
        {
            PlayDialogue(dialogue.nextDialogue[0]);
        }
        else if (!dialogue.nextDialogueIsNPC && dialogue.playerDialogueOptions.Length > 0)
        {
            dialogueBox.enabled = false;
            LockCursor(false);
            List<Button> buttons = new List<Button>();
            foreach (PlayerDialogueOption option in dialogue.playerDialogueOptions)
            {
                Button button = Instantiate(option.button, dialogueCanvas.transform);
                buttons.Add(button);
                button.onClick.AddListener(() => ClickButton(option.nextDialogue, buttons));
            }
        }
        else
        {
            ToggleDialogue(null);
        }
    }

    private void ClickButton(NPCDialogue dialogue, List<Button> buttons)
    {
        LockCursor(true);
        dialogueBox.enabled = true;
        foreach (Button button in buttons) Destroy(button.gameObject);
        PlayDialogue(dialogue);
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



