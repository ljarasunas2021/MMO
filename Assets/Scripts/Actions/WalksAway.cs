using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalksAway : Action1
{
    public Canvas canvas;
    public GameObject report;
    private GameObject reportInstant;
    public float missionShowTime;
    public Transform position;
    public GameObject dialogueBox;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public override IEnumerator Execute()
    {
        dialogueBox.SetActive(false);

        yield return 0;

        agent.SetDestination(position.position);

        reportInstant = Instantiate(report, canvas.transform);

        yield return new WaitForSeconds(missionShowTime);

        Destroy(reportInstant);

        dialogueBox.SetActive(true);
    }
}
