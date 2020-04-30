using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    public Dialogue[] dialogue;

    [Range(0, 1)]
    public float rotateSpeed;

    public float radius;

    private Outline outline;
    private UIManager UIScript;
    private CharacterController cc;
    private float veloY = 0;
    private int currentDialogIndex = 0;

    void Start()
    {
        UIScript = GameObject.FindObjectOfType<UIManager>();
        outline = GetComponent<Outline>();
        outline.enabled = false;
        cc = GetComponent<CharacterController>();
    }

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

    void OnMouseOver()
    {
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

    void OnMouseExit()
    {
        outline.enabled = false;
    }

    private bool PlayerCloseEnough()
    {
        float playerDist = Vector3.Distance(NetworkClient.connection.identity.transform.position, gameObject.transform.position);
        return (playerDist < radius);
    }

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

[System.Serializable]
public class Dialogue
{
    public NPCDialogue NPCDialogue;
    public PlayerDialogue playerDialogue;
}




