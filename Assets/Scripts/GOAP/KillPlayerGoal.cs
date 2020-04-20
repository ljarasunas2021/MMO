using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerGoal : GoapGoal
{
    public KillPlayerGoal(string key, float multiplier = 1) : base(key, multiplier)
    {

    }

    public override void UpdateMultiplier(DataSet data)
    {
        //this.multiplier = (data.data[GoapAction.Effects.PLAYER_DEAD + "0"]) ? 100 : 1;
    }
}
