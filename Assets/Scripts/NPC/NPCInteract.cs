using System.Collections;
using System.Collections.Generic;
using MMO;
using MMO.Player;
using MMO.UI;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

namespace MMO.NPC
{
    /// <summary> Interact with the NPC</summary>
    public class NPCInteract : Target
    {
        [Header("Dialogue")]
        // dialogue for the NPC
        public Dialogue[] dialogue;

        // how close the player must be to the npc to interact with it
        public float interactRadius;

        // npc's rotation speed
        [Range(0, 1)]
        public float rotateSpeed;

        // npc's character controller
        private CharacterController cc;

        // npc's Nav Mesh Agent script
        private NavMeshAgent navMeshAgent;

        // UIManager singleton reference
        private UIManager UIScript;

        // npc's animator
        private Animator anim;

        // current index for dialogue
        private int currentDialogIndex = 0;

        // Npc movement script
        private NPCMovement npcMovement;

        /// <summary> Init vars </summary>
        void Start()
        {
            UIScript = GameObject.FindObjectOfType<UIManager>();
            anim = GetComponent<Animator>();
            npcMovement = GetComponent<NPCMovement>();
        }

        /// <summary> Interact with the player </summary>
        public override void Interact()
        {
            if (IsPlayerCloseEnough())
            {
                if (UIManager.instance.canMove)
                {
                    SetInteracting(true);

                    StartCoroutine(RotateNPC());

                    UIScript.audioSource = GetComponent<AudioSource>();
                    UIScript.ToggleDialogue(dialogue[currentDialogIndex], this);

                    if (currentDialogIndex + 1 < dialogue.Length) currentDialogIndex++;
                }
            }
        }

        public void SetInteracting(bool val)
        {
            npcMovement.interacting = val;
        }

        /// <summary> Is the player within range of the NPC </summary>
        /// <returns> Whether the player is in range of the NPC </returns>
        private bool IsPlayerCloseEnough()
        {
            float playerDist = Vector3.Distance(NetworkClient.connection.identity.transform.position, gameObject.transform.position);
            return (playerDist < interactRadius);
        }

        /// <summary> Rotate the NPC </summary>
        /// <returns> an ienumerator since it is a coroutine, only use the ienumerator if you need information about the progress of a coroutine </returns>
        private IEnumerator RotateNPC()
        {
            float i = 0;
            while (i <= 1)
            {
                Quaternion playerPos = Quaternion.LookRotation(NetworkClient.connection.identity.transform.position - gameObject.transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, playerPos, i);
                i += rotateSpeed * Time.deltaTime;
                yield return 0;
            }
        }
    }

    /// <summary> Holds either a player or NPC dialogue </summary>
    [System.Serializable]
    public class Dialogue
    {
        public NPCDialogue NPCDialogue;
        public PlayerDialogue playerDialogue;
    }
}
