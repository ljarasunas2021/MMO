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
        if (!canMove) StartCoroutine(PlayDialogue(dialogue));
    }

    private IEnumerator PlayDialogue(NPCDialogue dialogue)
    {
        if (dialogue.action != null && dialogue.actionBeforeDialogue)
        {
            yield return StartCoroutine(dialogue.action.Execute());
        }

        dialogueText.text = dialogue.text;
        audioSource.PlayOneShot(dialogue.audio);

        StartCoroutine(WaitUntilAudioIsDone(dialogue));
    }

    private IEnumerator WaitUntilAudioIsDone(NPCDialogue dialogue)
    {
        yield return new WaitWhile(() => audioSource.isPlaying);

        if (dialogue.action != null && !dialogue.actionBeforeDialogue)
        {
            yield return StartCoroutine(dialogue.action.Execute());
        }

        if (dialogue.nextDialogueIsNPC && dialogue.nextDialogue != null)
        {
            StartCoroutine(PlayDialogue(dialogue.nextDialogue));
        }
        else if (!dialogue.nextDialogueIsNPC && dialogue.playerDialogueOptions.Length > 0)
        {
            dialogueBox.enabled = false;

            if (dialogue.playerDialogueOptions[0].action != null && dialogue.playerDialogueOptions[0].actionBeforeDialogue)
            {
                yield return StartCoroutine(dialogue.playerDialogueOptions[0].action.Execute());
            }

            LockCursor(false);
            List<Button> buttons = new List<Button>();
            foreach (PlayerDialogue option in dialogue.playerDialogueOptions)
            {
                Button button = Instantiate(option.button, dialogueCanvas.transform);
                buttons.Add(button);
                button.onClick.AddListener(() => StartCoroutine(ClickButton(option, option.nextDialogue, buttons)));
            }
        }
        else
        {
            ToggleDialogue(null);
        }
    }

    private IEnumerator ClickButton(PlayerDialogue optionChosen, NPCDialogue dialogue, List<Button> buttons)
    {
        LockCursor(true);

        if (optionChosen.action != null && !optionChosen.actionBeforeDialogue)
        {
            yield return StartCoroutine(optionChosen.action.Execute());
        }

        dialogueBox.enabled = true;
        foreach (Button button in buttons) Destroy(button.gameObject);
        StartCoroutine(PlayDialogue(dialogue));
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



