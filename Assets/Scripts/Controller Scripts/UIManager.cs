using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Manages the UI for the entire game</summary>
public class UIManager : MonoBehaviour
{
    // singleton
    public static UIManager instance;
    // dialogue text box
    public Text dialogueText;
    // the ____ canvas
    public Canvas pauseMenu, dialogueBox, compassCanvas, mapCanvas, dialogueCanvas;

    // source to play audio from
    [HideInInspector] public AudioSource audioSource;

    // the npc's dialogue
    private NPCDialogue dialogue;
    // the map
    private Map map;

    // toggle booleans
    [HideInInspector] public bool togglePauseMenu = false, toggleDialogueBox = false, toggleMap = false, toggleInventory = false;
    // abilities based on UI
    public bool canMove = true, canShoot = true;
    // should the dialogue be skipped
    private bool skipDialogue = false;

    /// <summary> Create singleton pattern, init vars, enable and disable appropriate objects </summary>
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("There is already an instance of the Audio Prefabs Controller.");
        }

        dialogueText = dialogueText.GetComponent<Text>();
        map = mapCanvas.GetComponent<Map>();
        pauseMenu.enabled = togglePauseMenu;
        dialogueBox.enabled = toggleDialogueBox;
        mapCanvas.enabled = toggleMap;
        compassCanvas.enabled = !toggleMap;

        UpdateCanShoot();
    }

    /// <summary> Toggle the audio on or off</summary>
    /// <param name="dialogue">the dialogue to play</param>
    public void ToggleDialogue(Dialogue dialogue)
    {
        canMove = (dialogue == null);
        dialogueBox.enabled = (dialogue != null);
        toggleDialogueBox = (dialogue != null);

        if (dialogue != null)
        {
            NPCDialogue NPCDialogue = dialogue.NPCDialogue;
            PlayerDialogue playerDialogue = dialogue.playerDialogue;

            if (NPCDialogue != null) StartCoroutine(PlayDialogue(NPCDialogue));
            else if (playerDialogue != null) StartCoroutine(PlayDialogue(playerDialogue));
            else Debug.Log("Dialogue Not Set Correctly");
        }

        UpdateCanShoot();
    }

    /// <summary> Play an NPC's dialogue</summary>
    /// <param name="dialogue"> the NPC's dialogue </param>
    /// <returns> an ienumerator since it is a coroutine, only use the ienumerator if you need information about the progress of a coroutine </returns>
    private IEnumerator PlayDialogue(NPCDialogue dialogue)
    {
        dialogueBox.enabled = true;

        if (dialogue.action != null && dialogue.actionBeforeDialogue)
        {
            yield return StartCoroutine(dialogue.action.Execute());
        }

        dialogueText.text = dialogue.text;
        audioSource.PlayOneShot(dialogue.audioClip);

        while(audioSource.isPlaying) {
            if (skipDialogue) {
                audioSource.Stop();
                skipDialogue = false;
            }
            yield return 0;
        }

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

    /// <summary> Play the player's dialogue</summary>
    /// <param name="dialogue"> the player's dialogue </param>
    /// <returns> an ienumerator since it is a coroutine, only use the ienumerator if you need information about the progress of a coroutine </returns>
    private IEnumerator PlayDialogue(PlayerDialogue dialogue)
    {
        dialogueBox.enabled = true;

        if (dialogue.action != null && dialogue.actionBeforeDialogue)
        {
            yield return StartCoroutine(dialogue.action.Execute());
        }

        dialogueText.text = dialogue.text;

        float dialogueCounter = 0;

        while(dialogueCounter < dialogue.time) {
            if (skipDialogue) {
                dialogueCounter = dialogue.time;
                skipDialogue = false;
            }
            dialogueCounter += Time.deltaTime;
            yield return 0;
        }
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

    /// <summary> Allow the player to choose a dialogue option </summary>
    /// <param name="dialogue"> an array of the player's dialogue options </param>
    /// <returns> an ienumerator since it is a coroutine, only use the ienumerator if you need information about the progress of a coroutine </returns>
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

        yield return 0;
    }

    /// <summary> Click one of the player's dialogue options </summary>
    /// <param name="optionChosen"> the dialogue option the player chose </param>
    /// <param name="dialogue"> the corresponding NPC's dialogue </param>
    /// <param name="buttons"> the list of dialogue option buttons </param>
    /// <returns> an ienumerator since it is a coroutine, only use the ienumerator if you need information about the progress of a coroutine </returns>
    private IEnumerator ClickButton(PlayerDialogue optionChosen, NPCDialogue dialogue, List<Button> buttons)
    {
        LockCursor(true);

        if (optionChosen.action != null && !optionChosen.actionBeforeDialogue)
        {
            yield return StartCoroutine(optionChosen.action.Execute());
        }

        dialogueBox.enabled = true;
        skipDialogue = false;
        foreach (Button button in buttons) Destroy(button.gameObject);
        StartCoroutine(PlayDialogue(dialogue));
    }

    /// <summary> Toggle the pause menu on or off </summary>
    public void TogglePauseMenu()
    {
        canMove = !canMove;
        togglePauseMenu = !togglePauseMenu;
        pauseMenu.enabled = !pauseMenu.enabled;

        UpdateCanShoot();
    }

    /// <summary> Toggle the map on or off </summary>
    public void ToggleMap()
    {
        toggleMap = !toggleMap;
        if (toggleMap) map.Enable();
        else map.Disable();
        mapCanvas.enabled = toggleMap;
        compassCanvas.enabled = !toggleMap;

        UpdateCanShoot();
    }

    /// <summary> Lock or unlock the cursor </summary>
    /// <param name="locked"> whether the cursor should be locked </param>
    public void LockCursor(bool locked)
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

    /// <summary> Turn the Inventory on or off </summary>
    public void ToggleInventory() {
        toggleInventory = !toggleInventory;
        InventoryManager.instance.ChangeEnabled();

        UpdateCanShoot();
    }

    /// <summary> Set can shoot to the appropriate value </summary>
    private void UpdateCanShoot() {
        canShoot = !toggleMap && !toggleDialogueBox && !togglePauseMenu && !toggleInventory;
    }

    /// <summary> Check if a dialogue can be skipped (if the player presses enter, when the dialogue box is up) </summary>
    public void CheckForSkipDialogue() {
        if (toggleDialogueBox) {
            skipDialogue = true;
        }
    }
}
