using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDialogueOption : MonoBehaviour
{
    [Header("Dialogue")]
    public Button button;
    public NPCDialogue nextDialogue;

    [Header("Action")]
    public Action1 action;
    public bool actionBeforeDialogue;
}
