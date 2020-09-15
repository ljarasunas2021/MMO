using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDialogue : MonoBehaviour
{
    [Header("Dialogue")]
    // should the player have options
    public bool option;
    // dialogue button
    public Button button;
    // dialogue text
    public string text;
    // dialogue time on screen
    public float time;

    [Header("Next Dialogue")]
    // is the next dialogue an npc
    public bool nextDialogueIsNPC;
    // if so this is the next dialogue
    public NPCDialogue nextDialogue;
    // if not should the player have dialogue options
    public bool options;
    // if so, heres the options
    public PlayerDialogue[] playerDialogueOptions;
    // otherwise, heres the players next dialogue
    public PlayerDialogue playerDialogue;

    [Header("Action")]
    // possible action
    public Action1 action;
    // should the action happen before or after the dialogue
    public bool actionBeforeDialogue;
}
