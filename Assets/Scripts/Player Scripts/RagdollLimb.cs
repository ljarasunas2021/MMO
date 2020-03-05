using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollLimb : MonoBehaviour
{
    private GameObject player;
    private Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        player = transform.root.gameObject;
        // if (player.GetComponent<BodyParts>().IsLocalPlayer())
        // {
        //     rigidbody.isKinematic = false;
        // }
    }
}
