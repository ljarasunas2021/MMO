using System.Collections;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [Range(0, 1)]
    public float speed;

    public float minFramesTillMove, maxFramesTillMove;

    public float moveRadius = 5;

    private Animator animator;
    private int framesTillMove;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("walking", false);
        SetFramesToMove();
    }

    void Update()
    {
        if (framesTillMove == 0)
        {
            float[] ranges = GetRange();
            Vector3 toMove = new Vector3(Random.Range(ranges[0], ranges[1]), transform.position.y, Random.Range(ranges[2], ranges[3]));
            StartCoroutine(MoveNPC(toMove));
        }

        framesTillMove--;
    }

    private float[] GetRange()
    {
        return new float[] { transform.position.x + moveRadius, transform.position.x - moveRadius, transform.position.z + moveRadius, transform.position.z - moveRadius };
    }

    private void SetFramesToMove()
    {
        framesTillMove = (int)Random.Range(minFramesTillMove, maxFramesTillMove);
    }

    private IEnumerator MoveNPC(Vector3 toMove)
    {
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
