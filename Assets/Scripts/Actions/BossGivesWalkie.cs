using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGivesWalkie : Action1
{
    public IEnumerator Execute()
    {
        yield return 0;
        Debug.Log("HERE");
    }
}
