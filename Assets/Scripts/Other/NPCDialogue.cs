using System.Collections;
using System.Collections.Generic;
using MMO.Player;
using UnityEngine;

namespace MMO.NPC
{
    /// <summary> Holds variables for an NPC's dialogue </summary>
    public class NPCDialogue : MonoBehaviour
    {
        // the words that will show up in the NPC's textbox when it speaks
        public string text;
        // the audio that plays when the NPC speaks
        public AudioClip audioClip;

        // will the next dialgoue be spoken by the NPC
        public bool nextDialogueIsNPC;
        // if nextDialogueIsNPC, this is the next dialogue
        public NPCDialogue nextDialogue;
        // if not nextDialogueIsNPC (meaning the next dialogue is the player), can the player choose from dialogue options
        public bool options;
        // if options, then there are the options the player can choose from
        public PlayerDialogue[] playerDialogueOptions;
        // if not options, then the player will just say this dialogue
        public PlayerDialogue playerDialogue;

        // if an action happens before or after dialogue
        public bool actionBeforeDialogue;
        // the action that happens
        public Action1 action;
    }
}
