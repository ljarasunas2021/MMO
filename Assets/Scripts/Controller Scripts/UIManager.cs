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

    public void ToggleDialogue(Dialogue dialogue)
    {
        canMove = (dialogue == null);
        dialogueBox.enabled = (dialogue != null);
        if (!canMove)
        {
            NPCDialogue NPCDialogue = dialogue.NPCDialogue;
            PlayerDialogue playerDialogue = dialogue.playerDialogue;

            if (NPCDialogue != null) StartCoroutine(PlayDialogue(NPCDialogue));
            else if (playerDialogue != null) StartCoroutine(PlayDialogue(playerDialogue));
            else Debug.Log("Dialogue Not Set Correctly");
        }
    }

    private IEnumerator PlayDialogue(NPCDialogue dialogue)
    {
        dialogueBox.enabled = true;

        if (dialogue.action != null && dialogue.actionBeforeDialogue)
        {
            yield return StartCoroutine(dialogue.action.Execute());
        }

        dialogueText.text = dialogue.text;
        audioSource.PlayOneShot(dialogue.audio);

        yield return new WaitWhile(() => audioSource.isPlaying);

        if (dialogue.action != null && !dialogue.actionBeforeDialogue)
        {
            yield return StartCoroutine(dialogue.action.Execute());
        }

        if (dialogue.nextDialogueIsNPC && dialogue.nextDialogue != null)
        {
            StartCoroutine(PlayDialogue(dialogue.nextDialogue));
        }
        else if (!dialogue.nextDialogueIsNPC && (dialogue.options && dialogue.playerDialogueOptions.Length > 0) || (!dialogue.options && dialogue.playerDialogue != null))
        {

            if (dialogue.options && dialogue.playerDialogueOptions.Length > 0)
            {
                StartCoroutine(PlayDialogue(dialogue.playerDialogueOptions));
            }
            else if (!dialogue.options && dialogue.playerDialogue != null)
            {
                StartCoroutine(PlayDialogue(dialogue.playerDialogue));
            }
        }
        else
        {
            ToggleDialogue(null);
        }
    }

    private IEnumerator PlayDialogue(PlayerDialogue dialogue)
    {
        dialogueBox.enabled = true;

        if (dialogue.action != null && dialogue.actionBeforeDialogue)
        {
            yield return StartCoroutine(dialogue.action.Execute());
        }

        dialogueText.text = dialogue.text;

        yield return new WaitForSeconds(dialogue.time);

        if (dialogue.action != null && !dialogue.actionBeforeDialogue)
        {
            yield return StartCoroutine(dialogue.action.Execute());
        }

        if (dialogue.nextDialogueIsNPC && dialogue.nextDialogue != null)
        {
            StartCoroutine(PlayDialogue(dialogue.nextDialogue));
        }
        else if (!dialogue.nextDialogueIsNPC && dialogue.playerDialogueOptions.Length > 0 || dialogue.playerDialogue != null)
        {
            if (dialogue.options && dialogue.playerDialogueOptions.Length > 0)
            {
                StartCoroutine(PlayDialogue(dialogue.playerDialogueOptions));
            }
            else if (!dialogue.options && dialogue.playerDialogue != null)
            {
                StartCoroutine(PlayDialogue(dialogue.playerDialogue));
            }
        }
        else
        {
            ToggleDialogue(null);
        }
    }

    private IEnumerator PlayDialogue(PlayerDialogue[] playerDialogueOptions)
    {
        dialogueBox.enabled = false;

        if (playerDialogueOptions[0].action != null && playerDialogueOptions[0].actionBeforeDialogue)
        {
            yield return StartCoroutine(playerDialogueOptions[0].action.Execute());
        }

        LockCursor(false);
        List<Button> buttons = new List<Button>();
        foreach (PlayerDialogue option in playerDialogueOptions)
        {
            Button button = Instantiate(option.button, dialogueCanvas.transform);
            buttons.Add(button);
            button.onClick.AddListener(() => StartCoroutine(ClickButton(option, option.nextDialogue, buttons)));
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



