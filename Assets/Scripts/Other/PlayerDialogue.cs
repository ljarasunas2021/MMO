using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Holds variables for an player's dialogue </summary>
public class PlayerDialogue : MonoBehaviour
{
    [Header("Dialogue")]
    // should the player have different dialogue options 
    public bool options;
    // if options, this is a button for a dialogue option
    public Button button;
    // if not options, this is the text that will be displayed for the player
    public string text;
    // the amount of time the text is on the screen
    public float time;

    [Header("Next Dialogue")]
    // does the NPC speek next
    public bool nextDialogueIsNPC;
    // if nextDialogueIsNPC, then this is the next dialogue that will be played
    public NPCDialogue nextDialogue;
    // if not nextDialogueIsNPC (meaning the player speaks next), will the player have dialogue options
    public bool nextOptions;
    // if nextOptions, heres the player dialogue options
    public PlayerDialogue[] playerDialogueOptions;
    // if not nextOptions, heres the player's next dialogue
    public PlayerDialogue playerDialogue;

    [Header("Action")]
    // should an action happen before or after the dialogue
    public bool actionBeforeDialogue;
    // the action that would happen
    public Action1 action;
}
