using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// add gravity to CC
public class Gravity : MonoBehaviour
{
    // gravity amount
    public float gravity = 0.015f;
    // y velocity of cc
    private float veloY;
    // cc variable
    private CharacterController cc;

    // get the cc
    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // add the gravity each frame
    private void Update()
    {
        AddGravity();
    }

    // add gravity appropriately
    private void AddGravity()
    {
        if (cc.isGrounded) veloY = 0;
        else veloY += gravity;

        cc.Move(new Vector3(0, -veloY, 0));

        if (cc.isGrounded) veloY = 0;
    }
}
