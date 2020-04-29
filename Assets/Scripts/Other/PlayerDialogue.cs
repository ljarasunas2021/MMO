using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDialogue : MonoBehaviour
{
    [Header("Dialogue")]
    public bool option;
    public Button button;
    public string text;
    public float time;

    [Header("Next Dialogue")]
    public bool nextDialogueIsNPC;
    public NPCDialogue nextDialogue;
    public bool options;
    public PlayerDialogue[] playerDialogueOptions;
    public PlayerDialogue playerDialogue;

    [Header("Action")]
    public Action1 action;
    public bool actionBeforeDialogue;
}
