using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public string text;
    public AudioClip audio;
    public bool nextDialogueIsNPC;
    public NPCDialogue[] nextDialogue;
    public Action1 action;
    public bool actionBeforeDialogue;
    public PlayerDialogueOption[] playerDialogueOptions;
}
