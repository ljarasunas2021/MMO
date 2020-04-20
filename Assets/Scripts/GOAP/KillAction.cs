using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AI.Actions
{
    public class KillAction : GoapAction
    {
        private int targetPlayerId;
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
            agent.GetComponent<EnemyAI1>().itemHolding.weaponScript.Fire();

            Vector3 dir = (target.transform.position - agent.transform.position).normalized;

            Quaternion lookRot = Quaternion.LookRotation(dir);

            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRot, Time.deltaTime * 20);
        }

        public override GoapAction Clone()
        {
            return new KillAction(agent, targetPlayerId).SetClone(originalObjectGUID);
        }
    }
}