using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMO.Actions
{
    public class Pause : Action1
    {
        public float time = 2;

        public override IEnumerator Execute()
        {
            yield return new WaitForSeconds(time);
        }
    }
}
