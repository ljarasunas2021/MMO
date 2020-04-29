using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue")]
    public string text;
    public AudioClip audio;

    [Header("Next Dialogue")]
    public bool nextDialogueIsNPC;
    public NPCDialogue nextDialogue;
    public PlayerDialogueOption[] playerDialogueOptions;

    [Header("Action")]
    public Action1 action;
    public bool actionBeforeDialogue;
}
