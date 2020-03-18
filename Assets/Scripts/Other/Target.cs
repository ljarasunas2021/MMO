using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    public NPCDialogue dialogue;

    [Range(0, 1)]
    public float rotateSpeed;

    public float radius;

    public float gravity;

    private Outline outline;
    private UIManager UIScript;
    private CharacterController cc;
    private float veloY = 0;

    void Start()
    {
        UIScript = GameObject.FindObjectOfType<UIManager>();
        outline = GetComponent<Outline>();
        outline.enabled = false;
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        AddGravity();
    }

    private void AddGravity()
    {
        if (cc.isGrounded) veloY = 0;
        else veloY += gravity;

        cc.Move(new Vector3(0, -veloY, 0));

        if (cc.isGrounded) veloY = 0;
    }

    public void Interact()
    {
        if (PlayerCloseEnough())
        {
            if (UIManager.canMove)
            {
                StartCoroutine(RotateNPC());

                UIScript.audioSource = GetComponent<AudioSource>();
                UIScript.ToggleDialogue(dialogue);
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
public class NPCDialogue
{
    public string text;
    public AudioClip audio;
    public bool nextDialogueIsNPC;
    public NPCDialogue[] nextDialogue;
    public PlayerDialogueOption[] playerDialogueOptions;
}

[System.Serializable]
public class PlayerDialogueOption
{
    public Button button;
    public NPCDialogue nextDialogue;
}




