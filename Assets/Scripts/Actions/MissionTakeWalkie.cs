using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionTakeWalkie : Action1
{
    public IEnumerator Execute()
    {
        yield return 0;
        Debug.Log("HERE");
    }
}
