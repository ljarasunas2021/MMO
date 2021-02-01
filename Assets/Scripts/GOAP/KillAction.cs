using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SwordGC.AI.Actions
{
    /// <summary> A killing action for the GOAP AI. To learn about GOAP AI, visit https://gamedevelopment.tutsplus.com/tutorials/goal-oriented-action-planning-for-a-smarter-ai--cms-20793  </summary>
    public class KillAction : GoapAction
    {
        // the player id of the target
        private int targetPlayerId;
        // the minimum countdown to shoot
        private int minCountdown = 10;
        // the maximum countdown to shoot
        private int  maxCountdown = 30;
        // the amount of time until the AI can shoot
        private int  timeTillCanShoot = 0;
        // the goap agent itself
        private GoapAgent agent;

        /// <summary> Contstructor for the kill action </summary>
        /// <param name="agent"> agent that will execute this action </param>
        /// <param name="targetPlayerId"> player id of the target of this action </param>
        /// <returns> the KillAction object created </returns>
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

        /// <summary> Perform the kill action </summary>
        public override void Perform()
        {
            if (timeTillCanShoot <= 0)
            {
                agent.GetComponent<EnemyAI1>().itemHolding.weaponScript.Fire();
                timeTillCanShoot = Random.Range(minCountdown, maxCountdown);
            }

            timeTillCanShoot--;
        }

        /// <summary> Clone and return this GOAP action </summary>
        /// <returns> the clone </returns>
        public override GoapAction Clone()
        {
            return new KillAction(agent, targetPlayerId).SetClone(originalObjectGUID);
        }
    }
}