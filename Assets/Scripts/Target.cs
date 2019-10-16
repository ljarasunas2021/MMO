using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Target : MonoBehaviour
{
    private Outline outline;
    private UIManager UIScript;
    public string[] dialogue;
    public AudioClip[] inputSounds;

    public string interactKey;

    private float radius = 6f;

    void Start()
    {
        UIScript = GameObject.FindObjectOfType<UIManager>();
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void Interact()
    {
        if (PlayerCloseEnough())
        {
            switch (interactKey)
            {
                case "npc":
                    if (UIScript.canMove)
                    {
                        //{
                        // Vector3 playerPos = Vector3.Slerp(gameObject.transform.position, NetworkClient.connection.identity.transform.position, .05f);
                        // transform.LookAt(playerPos);
                        StartCoroutine(RotateNPC());

                        UIScript.ToggleDialogueBox(dialogue, inputSounds);

                    }
                    break;
                case "device":
                    break;
            }
        }
    }

    void OnMouseOver()
    {
        if (PlayerCloseEnough())
        {
            if (UIScript.canMove && !outline.enabled)
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
        if (playerDist < radius)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator RotateNPC()
    {
        float i = 0;
        while (i <= 20)
        {
            // Vector3 playerPos = Vector3.Slerp(gameObject.transform.position, NetworkClient.connection.identity.transform.position, .05f);
            // gameObject.transform.LookAt(playerPos);
            Quaternion playerPos = Quaternion.LookRotation(NetworkClient.connection.identity.transform.position - gameObject.transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, playerPos, i);
            i += .05f;
            yield return 0;
        }
    }
}
