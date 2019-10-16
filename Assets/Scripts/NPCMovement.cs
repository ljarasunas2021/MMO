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
        animator = GetComponent<Animator>();
        animator.SetBool("walking", false);
        toMove = transform.position;
    }

    void Update() {
        int random = Random.Range(1, 1000);
        if (random <= 2 && !moving) {
            toMove = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minZ, maxZ));
            Debug.Log("toMove = " + toMove);
            moving = true;
            animator.SetBool("walking", true);
            transform.LookAt(toMove);
            transform.position = Vector3.Slerp(transform.position, toMove, .1f);
            StartCoroutine(MoveNPC());
        }
    }

    private void GetRange() {
        maxX = transform.position.x + radius;
        minX = transform.position.x - radius;
        maxZ = transform.position.z + radius;
        minZ = transform.position.z - radius;
    }

    private IEnumerator MoveNPC() {
        float i = 0;
        while (i<=1) {
            transform.position = Vector3.Lerp(transform.position, toMove, i*Time.deltaTime);
            i += .01f;
            yield return 0;
        }
        moving = false;
        animator.SetBool("walking", false);
        Debug.Log("moving set to false");
    }
}
