using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDialogueOption : MonoBehaviour
{
    public Button button;
    public NPCDialogue nextDialogue;
    public Action1 action;
    public bool actionBeforeDialogue;
}
