using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionTakeWalkie : Action1
{
    public Canvas canvas;
    public GameObject mission;
    public float time;
    private GameObject missionInstant;
    public GameObject dialogueBox;

    public override IEnumerator Execute()
    {
        dialogueBox.SetActive(false);
        missionInstant = Instantiate(mission, canvas.transform);
        yield return new WaitForSeconds(time);
        Destroy(missionInstant);
        dialogueBox.SetActive(true);
    }
}
