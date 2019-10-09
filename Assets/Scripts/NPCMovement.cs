using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour {
    private float maxX;
    private float minX;
    private float maxY;
    private float minY;
    private float radius;
    private bool moving = false;
    private CharacterController cc;
    private Vector3 toMove;
    private float speed = 5f;

    void Start() {
        GetRange();
        cc = gameObject.GetComponent<CharacterController>();
        toMove = transform.position;
    }

    void Update() {
        int random = Random.Range(1, 1000);
        if (random <= 2 && !moving) {
            toMove = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minY, maxY));
            // cc.Move(toMove);
            transform.Translate(toMove);
            moving = true;
        }
        if (moving) {
            transform.Translate(toMove);
            if (transform.position == toMove) {
                moving = false;
            }
        }
    }

    private void GetRange() {
        maxX = transform.position.x + radius;
        minX = transform.position.x - radius;
        maxY = transform.position.y + radius;
        minY = transform.position.y - radius;
    }

    // private void MoveNPC() {

    // }
}
