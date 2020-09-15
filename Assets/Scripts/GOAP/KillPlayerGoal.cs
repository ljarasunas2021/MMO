using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerGoal : GoapGoal
{
    // goal to kill player
    public KillPlayerGoal(string key, float multiplier = 1) : base(key, multiplier)
    {

    }

    // update multiplier (does nothing now)
    public override void UpdateMultiplier(DataSet data)
    {
        //this.multiplier = (data.data[GoapAction.Effects.PLAYER_DEAD + "0"]) ? 100 : 1;
    }
}
