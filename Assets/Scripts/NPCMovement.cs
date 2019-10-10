using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour {
    private float maxX;
    private float minX;
    private float maxZ;
    private float minZ;
    private float radius = 5;
    private bool moving = false;
    private CharacterController cc;
    private Vector3 toMove;
    private float speed = 5f;
    private Animator animator;

    void Start() {
        GetRange();
       // cc = gameObject.GetComponent<CharacterController>();
       animator = GetComponent<Animator>();
       animator.SetBool("walking", false);
        toMove = transform.position;
    }

    void Update() {
        int random = Random.Range(1, 100);
        if (random <= 2 && !moving) {
            toMove = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minZ, maxZ));
            Debug.Log("toMove = " + toMove);
            // cc.Move(toMove);
            //transform.Translate(toMove);
            moving = true;
            animator.SetBool("walking", true);
            // Vector3 facing = Vector3.RotateTowards(transform.forward, toMove, 0.0f, 0.0f);
            // transform.rotation = Quaternion.LookRotation(facing);
            transform.LookAt(toMove);
            transform.position = Vector3.Slerp(transform.position, toMove, .1f);
            StartCoroutine(MoveNPC());
        }
        // if (moving) {
        //    // transform.Translate(toMove*Time.deltaTime);
        //     transform.position = Vector3.Slerp(transform.position, toMove, .1f);
        //     //Vector3 tarPos = new Vector3(toMove.x, transform.position.y, toMove.z);
        //     StartCoroutine(Timer(5));
        // }
    }

    private void GetRange() {
        maxX = transform.position.x + radius;
        minX = transform.position.x - radius;
        maxZ = transform.position.z + radius;
        minZ = transform.position.z - radius;
        //Debug.Log(gameObject.name + "'s movement range" + maxX + ", " + minX + ", " + maxZ + ", " + minZ);
    }

    private IEnumerator MoveNPC() {
        // yield return new WaitForSeconds(seconds);
        // moving = false;
        // Debug.Log("moving: " + moving);

        float i = 0;
        while (i<=1) {
            // Vector3 playerPos = Vector3.Slerp(gameObject.transform.position, NetworkClient.connection.identity.transform.position, .05f);
            // gameObject.transform.LookAt(playerPos);
            //Quaternion playerPos = Quaternion.LookRotation(NetworkClient.connection.identity.transform.position - gameObject.transform.position);
            transform.position = Vector3.Lerp(transform.position, toMove, i);
            //transform.rotation = Quaternion.Slerp(transform.rotation, playerPos, i);
            i += .001f;
            yield return 0;
        }
        moving = false;
        animator.SetBool("walking", false);
        Debug.Log("moving set to false");
    }
}
