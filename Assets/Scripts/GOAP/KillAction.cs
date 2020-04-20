using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AI.Actions
{
    public class KillAction : GoapAction
    {
        private int targetPlayerId, minCountdown = 10, maxCountdown = 30, timeTillCanShoot = 0;
        private GoapAgent agent;

        public KillAction(GoapAgent agent, int targetPlayerId) : base(agent)
        {
            this.agent = agent;
            goal = GoapGoal.Goals.KILL_PLAYER; //+ targetPlayerId;
            preconditions.Add(Effects.PLAYER_DEAD + targetPlayerId, false);

            requiredRange = 1000f;
            cost = 20;

            this.targetPlayerId = targetPlayerId;

            targetString = "Player_" + targetPlayerId + "(Clone)";
        }

        public override void Perform()
        {
            if (timeTillCanShoot <= 0)
            {
                agent.GetComponent<EnemyAI1>().itemHolding.weaponScript.Fire();
                timeTillCanShoot = Random.Range(minCountdown, maxCountdown);
            }

            timeTillCanShoot--;
        }

        public override GoapAction Clone()
        {
            return new KillAction(agent, targetPlayerId).SetClone(originalObjectGUID);
        }
    }
}