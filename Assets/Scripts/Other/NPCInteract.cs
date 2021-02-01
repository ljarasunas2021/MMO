using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Interact with the NPC</summary>
public class NPCInteract : Target
{
    // dialogue for the NPC
    public Dialogue[] dialogue;

    // npc's rotation speed
    [Range(0, 1)]
    public float rotateSpeed;

    // how close the player must be to the npc to interact with it
    public float radius;

    // npc's character controller
    private CharacterController cc;

    // UIManager singleton reference
    private UIManager UIScript;
    
    // y velocity of NPC
    private float veloY = 0;
    // current index for dialogue
    private int currentDialogIndex = 0;

    /// <summary> Init vars </summary>
    void Start()
    {
        UIScript = GameObject.FindObjectOfType<UIManager>();
        cc = GetComponent<CharacterController>();
    }

    /// <summary> Interact with the player </summary>
    public override void Interact()
    {
        if (IsPlayerCloseEnough())
        {
            if (UIManager.instance.canMove)
            {
                StartCoroutine(RotateNPC());

                UIScript.audioSource = GetComponent<AudioSource>();
                UIScript.ToggleDialogue(dialogue[currentDialogIndex]);

                if (currentDialogIndex + 1 < dialogue.Length) currentDialogIndex++;
            }
        }
    }

    /// <summary> Is the player within range of the NPC </summary>
    /// <returns> Whether the player is in range of the NPC </returns>
    private bool IsPlayerCloseEnough()
    {
        float playerDist = Vector3.Distance(NetworkClient.connection.identity.transform.position, gameObject.transform.position);
        return (playerDist < radius);
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




