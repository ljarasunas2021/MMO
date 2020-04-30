using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : Action1
{

    public float time = 2;

    public new IEnumerator Execute()
    {
        yield return new WaitForSeconds(time);
    }
}
