using System.Collections;
using System.Collections.Generic;
using MMO;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

/// <summary> Interact with the NPC</summary>
public class NPCInteract : Target
{
    [Header("Dialogue")]
    // dialogue for the NPC
    public Dialogue[] dialogue;

    // how close the player must be to the npc to interact with it
    public float interactRadius;

    [Header("Movement")]
    // Should the npc not move
    public bool stationary = true;

    // if not stationary, how far should the npc move each time
    public float moveDistance;

    // is the npc interacting
    [HideInInspector] public bool interacting = false;

    // interacting last frame
    private bool interactingLastFrame = false;

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
    
    // y velocity of NPC
    // private float veloY = 0;
    // current index for dialogue
    private int currentDialogIndex = 0;

    /// <summary> Init vars </summary>
    void Start()
    {
        UIScript = GameObject.FindObjectOfType<UIManager>();
        cc = GetComponent<CharacterController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        SetNewDestination();
        anim.SetBool("walking", true);
    }

    /// <summary> Interact with the player </summary>
    public override void Interact()
    {
        if (IsPlayerCloseEnough())
        {
            if (UIManager.instance.canMove)
            {
                interacting = true;

                StartCoroutine(RotateNPC());

                UIScript.audioSource = GetComponent<AudioSource>();
                UIScript.ToggleDialogue(dialogue[currentDialogIndex], this);

                if (currentDialogIndex + 1 < dialogue.Length) currentDialogIndex++;
            }
        }
    }

    private void Update()
    {
        if (!stationary)
        {
            if (!interacting)
            {
                if (interactingLastFrame)
                {
                    navMeshAgent.isStopped = false;
                    anim.SetBool("walking", true);
                }
                if (Vector3.Distance(transform.position, navMeshAgent.destination) < 5)
                {
                    SetNewDestination();
                }
            }
            else
            {
                if (!interactingLastFrame)
                {
                    navMeshAgent.isStopped = true;
                    anim.SetBool("walking", false);
                }
            }
            interactingLastFrame = interacting;
        }
    }

    private void SetNewDestination()
    {
        navMeshAgent.SetDestination(RandomNavSphere(transform.position, moveDistance, -1));
    }

    private Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
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




