using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMO.Actions
{
    public class BossGivesWalkie : Action1
    {
        public GameObject walkie;
        public Transform position;

        public override IEnumerator Execute()
        {
            Instantiate(walkie, position.position, Quaternion.identity);
            yield break;
        }
    }
}
