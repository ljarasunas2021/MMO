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
        // animator component of npc
        private Animator anim;
        // nevmeshagent component of npc
        private NavMeshAgent navMeshAgent;
        // actual frames till npc moves
        private int framesTillMove;
        // player transform
        private Transform playerTransform;

        // Should the npc not move
        public bool stationary = true;

        // if not stationary, how far should the npc move each time
        public float moveDistance;

        // is the npc interacting
        [HideInInspector] public bool interacting = false;

        // interacting last frame
        private bool interactingLastFrame = false;

        /// <summary> Init vars </summary>
        void Start()
        {
            anim = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();

            if (!stationary)
            {
                anim.SetBool("walking", true);
                
                if (!attackMode)
                {
                    SetNewDestination();
                }
            }                      
        }

        /// <summary> Move the NPC if necessary </summary>
        void Update()
        {
            if (!stationary)
            {
                if (!interacting)
                {
                    if (interactingLastFrame)
                    {
                        navMeshAgent.isStopped = false;
                        anim.SetBool("walking", true);
                    }
                    if (!attackMode && Vector3.Distance(transform.position, navMeshAgent.destination) < 5)
                    {
                        SetNewDestination();
                    }
                }
                else
                {
                    if (!interactingLastFrame)
                    {
                        navMeshAgent.isStopped = true;
                        anim.SetBool("walking", false);
                    }
                }

                if (attackMode)
                {
                    if (playerTransform == null) playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
                    else navMeshAgent.SetDestination(playerTransform.position);
                }

                interactingLastFrame = interacting;
            }
        }

        // set roaming nav mesh destination
        private void SetNewDestination()
        {
            navMeshAgent.SetDestination(RandomNavSphere(transform.position, moveDistance, -1));
        }

        // find random nav mesh position
        private Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

            randomDirection += origin;

            NavMeshHit navHit;

            NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

            return navHit.position;
        }
    }
}
