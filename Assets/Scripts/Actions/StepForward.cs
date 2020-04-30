using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepForward : Action1
{
    public float forward;

    public new IEnumerator Execute()
    {
        yield return 0;
        transform.position += transform.forward * forward;
    }
}
