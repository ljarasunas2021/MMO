using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalksAway : Action1
{
    // also show MISSION
    public IEnumerator Execute()
    {
        yield return 0;
        Debug.Log("HERE");
    }
}
