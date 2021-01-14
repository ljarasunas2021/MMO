using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace MMO.NPC
{
    /// <summary> Controls the movement of the NPC </summary>
    public class NPCMovement : MonoBehaviour
    {
        // attack or idle?
        public bool attackMode;
        // NPC's speed
        [Range(0, 1)]
        public float speed;
        // min/max frames till npc starts moving to its next position
        public float minFramesTillMove, maxFramesTillMove;
        // radius within which npc can move
        public float moveRadius = 5;
        // animator component of npc
        private Animator animator;
        // nevmeshagent component of npc
        private NavMeshAgent navMeshAgent;
        // actual frames till npc moves
        private int framesTillMove;
        // player transform
        private Transform playerTransform;

        /// <summary> Init vars </summary>
        void Start()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator.SetBool("walking", false);
            SetFramesToMove();
        }

        /// <summary> Move the NPC if necessary </summary>
        void Update()
        {
            if (attackMode)
            {
                animator.speed = 3;
                animator.SetBool("walking", true);
                if (playerTransform == null) playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
                else navMeshAgent.SetDestination(playerTransform.position);
            }
            else
            {
                if (framesTillMove < 0) StartCoroutine(MoveNPC());

                framesTillMove--;
            }
        }

        /// <summary> Find the bounds within which the NPC can move</summary>
        /// <returns> a float array of the form [max X position, min X position, max Z position, min Z position] </returns>
        private float[] GetMovementBounds()
        {
            return new float[] { transform.position.x + moveRadius, transform.position.x - moveRadius, transform.position.z + moveRadius, transform.position.z - moveRadius };
        }

        /// <summary> Set the frames till move variable </summary>
        private void SetFramesToMove()
        {
            framesTillMove = (int)Random.Range(minFramesTillMove, maxFramesTillMove);
        }

        /// <summary> Move the NPC</summary>
        /// <returns> an ienumerator since it is a coroutine, only use the ienumerator if you need information about the progress of a coroutine </returns>
        private IEnumerator MoveNPC()
        {
            float[] ranges = GetMovementBounds();
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
}
