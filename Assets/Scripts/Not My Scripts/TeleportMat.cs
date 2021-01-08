using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportMat : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
           CharacterController cc = other.gameObject.GetComponent<CharacterController>();
           cc.enabled = false;
           cc.transform.Translate(0,16,0);
           cc.enabled = true;
        }
    }
}
