using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> The kill player goal for the GOAP Agent </summary>
public class KillPlayerGoal : GoapGoal
{
    /// <summary> Constructor for the KillPlayerGoal </summary>
    /// <param name="key"> unique name of the goal </param>
    /// <param name="multiplier"> The cost multiplier. Can be used to create a tendency towards a goal </param>
    /// <returns> the KillPlayerGoal object </returns>
    public KillPlayerGoal(string key, float multiplier = 1) : base(key, multiplier)
    {

    }

    /// <summary> Update the multiplier (does nothing now) </summary>
    /// <param name="data"> the collective dataset for all AI </param>
    public override void UpdateMultiplier(DataSet data)
    {
        //this.multiplier = (data.data[GoapAction.Effects.PLAYER_DEAD + "0"]) ? 100 : 1;
    }
}
