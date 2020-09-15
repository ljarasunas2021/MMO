using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AI.Actions
{
    // killing action
    public class KillAction : GoapAction
    {
        // target player id, shooting varibales
        private int targetPlayerId, minCountdown = 10, maxCountdown = 30, timeTillCanShoot = 0;
        // agent
        private GoapAgent agent;

        // def of kill action
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

        // preform kill action
        public override void Perform()
        {
            if (timeTillCanShoot <= 0)
            {
                agent.GetComponent<EnemyAI1>().itemHolding.weaponScript.Fire();
                timeTillCanShoot = Random.Range(minCountdown, maxCountdown);
            }

            timeTillCanShoot--;
        }

        // clone action
        public override GoapAction Clone()
        {
            return new KillAction(agent, targetPlayerId).SetClone(originalObjectGUID);
        }
    }
}