using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalksAway : Action1
{
    public Canvas canvas;
    public GameObject report;
    private GameObject reportInstant;
    public float missionShowTime;

    public IEnumerator Execute()
    {
        yield return 0;
        Debug.Log("Implement Walk Away");

        reportInstant = Instantiate(report, canvas.transform);

        yield return new WaitForSeconds(missionShowTime);

        Destroy(reportInstant);
    }
}
