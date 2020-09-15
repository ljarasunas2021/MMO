using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// dialogue for npc
public class NPCDialogue : MonoBehaviour
{
    // what npc will say (text + audio)
    public string text;
    public AudioClip audio;

    // npc will speak after this one
    public bool nextDialogueIsNPC;
    // the next dialogue (if is npc)
    public NPCDialogue nextDialogue;
    // if the player speaks next, will the player be presented with options
    public bool options;
    // if so these are the options
    public PlayerDialogue[] playerDialogueOptions;
    // if not then the player will just say this
    public PlayerDialogue playerDialogue;

    // action that happens
    public Action1 action;
    // if action happends before or after dialogue
    public bool actionBeforeDialogue;
}
