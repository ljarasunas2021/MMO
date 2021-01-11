using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary> Adds gravity to cc </summary>
[RequireComponent(typeof(CharacterController))]
public class Gravity : MonoBehaviour
{
    // gravity amount
    public float gravity = 0.015f;
    // y velocity of cc
    private float veloY;
    // cc variable
    private CharacterController cc;
    // navmesh agent
    private NavMeshAgent navMeshAgent;

    /// <summary> Get the CC </summary>
    private void Start()
    {
        cc = GetComponent<CharacterController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    /// <summary> Add gravity each frame </summary>
    private void Update()
    {
        AddGravity();
    }

    /// <summary> Add appropriate gravity </summary>
    private void AddGravity()
    {
        if (cc.isGrounded) veloY = 0;
        else veloY += gravity;

        if (navMeshAgent == null)
        {
            cc.Move(new Vector3(0, -veloY, 0));
        } else
        {
            navMeshAgent.velocity += new Vector3(0, -veloY, 0);
        }

        if (cc.isGrounded) veloY = 0;
    }
}
