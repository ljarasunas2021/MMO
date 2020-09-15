using System.Collections;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    // speed of npc
    [Range(0, 1)]
    public float speed;
    // min/max frames till npc moves
    public float minFramesTillMove, maxFramesTillMove;
    // radius within which npc can move
    public float moveRadius = 5;
    // animator of npc
    private Animator animator;
    // actual frames till npc moves
    private int framesTillMove;

    // start npc
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("walking", false);
        SetFramesToMove();
    }

    // move if necessary
    void Update()
    {
        if (framesTillMove < 0) StartCoroutine(MoveNPC());

        framesTillMove--;
    }

    // get max and min x, max and min z
    private float[] GetRange()
    {
        return new float[] { transform.position.x + moveRadius, transform.position.x - moveRadius, transform.position.z + moveRadius, transform.position.z - moveRadius };
    }

    // get randome grames to move
    private void SetFramesToMove()
    {
        framesTillMove = (int)Random.Range(minFramesTillMove, maxFramesTillMove);
    }

    // move npc
    private IEnumerator MoveNPC()
    {
        float[] ranges = GetRange();
        Vector3 toMove = new Vector3(Random.Range(ranges[0], ranges[1]), transform.position.y, Random.Range(ranges[2], ranges[3]));
        framesTillMove = int.MaxValue;

        animator.SetBool("walking", true);
        transform.LookAt(toMove);

        float i = 0;
        while (i <= 1)
        {
            transform.position = Vector3.Lerp(transform.position, toMove, i * Time.deltaTime);
            i += speed;
            yield return 0;
        }

        animator.SetBool("walking", false);
        SetFramesToMove();
    }
}
