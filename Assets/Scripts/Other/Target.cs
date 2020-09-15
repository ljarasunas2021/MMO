using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    // target would be outlined
    public bool useOutline = true;
    // dialogue
    public Dialogue[] dialogue;

    // speed of rotation
    [Range(0, 1)]
    public float rotateSpeed;

    // radius for player
    public float radius;

    // components
    private Outline outline;
    private UIManager UIScript;
    private CharacterController cc;

    // y rotation velocity
    private float veloY = 0;
    // current dialogue index
    private int currentDialogIndex = 0;

    // get ready
    void Start()
    {
        UIScript = GameObject.FindObjectOfType<UIManager>();
        outline = GetComponent<Outline>();
        outline.enabled = false;
        cc = GetComponent<CharacterController>();
    }

    // interact with player if necessary
    public void Interact()
    {
        if (PlayerCloseEnough())
        {
            if (UIManager.canMove)
            {
                StartCoroutine(RotateNPC());

                UIScript.audioSource = GetComponent<AudioSource>();
                UIScript.ToggleDialogue(dialogue[currentDialogIndex]);

                if (currentDialogIndex + 1 < dialogue.Length) currentDialogIndex++;
            }
        }
    }

    // enable outline if neccessary
    void OnMouseOver()
    {
        if (!useOutline) return;

        if (PlayerCloseEnough())
        {
            if (UIManager.canMove && !outline.enabled)
            {
                outline.enabled = true;
            }
        }
        else
        {
            outline.enabled = false;
        }
    }

    // disable outline once mouse exits
    void OnMouseExit()
    {
        outline.enabled = false;
    }

    // is the player within range
    private bool PlayerCloseEnough()
    {
        float playerDist = Vector3.Distance(NetworkClient.connection.identity.transform.position, gameObject.transform.position);
        return (playerDist < radius);
    }

    // rotate the npc
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

// either the player or npc dialogue
[System.Serializable]
public class Dialogue
{
    public NPCDialogue NPCDialogue;
    public PlayerDialogue playerDialogue;
}




