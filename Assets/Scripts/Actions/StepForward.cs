using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepForward : Action1
{
    public float forward;
    private CharacterController cc;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    public override IEnumerator Execute()
    {
        yield return 0;
        cc.Move(transform.forward * forward);
    }
}
