using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Target : MonoBehaviour
{
    public Dialogue[] dialogue;

    [Range(0, 1)]
    public float rotateSpeed;

    public string interactKey;

    private Outline outline;
    private UIManager UIScript;
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
                        StartCoroutine(RotateNPC());

                        UIScript.ToggleDialogue(dialogue);
                    }
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
    public string text;
    public AudioClip audio;
}
