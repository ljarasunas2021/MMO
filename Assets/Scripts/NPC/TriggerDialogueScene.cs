using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MMO.NPC;

namespace MMO.NPC
{
    public class TriggerDialogueScene : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            GetComponent<DialogueScene>().StartDialogue(other.gameObject);
        }
    }
}

