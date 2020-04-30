using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionTakeWalkie : Action1
{
    public Canvas canvas;
    public GameObject mission;
    public float time;
    private GameObject missionInstant;

    public new IEnumerator Execute()
    {
        missionInstant = Instantiate(mission, canvas.transform);
        yield return new WaitForSeconds(time);
        Destroy(missionInstant);
    }
}
