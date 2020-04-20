using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public float gravity = 0.015f;
    private float veloY;
    private CharacterController cc;

    private void Start()
    {
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
}
