using UnityEngine;
using System.Collections;
using System;
using Mirror;

public class RagdollController : NetworkBehaviour
{
    public Rigidbody[] bodyParts;

    private Animator animator;
    private CharacterController characterController;

    private void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    [Command]
    public void CmdBecomeRagdoll()
    {
        animator.enabled = false;
        characterController.detectCollisions = false;
        foreach (Rigidbody bodyPart in bodyParts)
        {
            bodyPart.isKinematic = false;
        }
    }
}
