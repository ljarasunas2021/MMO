using System.Collections;
using System.Collections.Generic;
using Mirror;
using SwordGC.AI.Actions;
using SwordGC.AI.Goap;
using UnityEngine;

///<summary> GOAP AI system for enemy 1. To learn about GOAP AI, visit https://gamedevelopment.tutsplus.com/tutorials/goal-oriented-action-planning-for-a-smarter-ai--cms-20793 </summary>
public class EnemyAI1 : GoapAgent
{
    // index of current weapon the enemy is holding
    public int weaponIndex;
    // PID controller for enemy's aim system
    public float aimP, aimI, aimD;
    // right hand gameobject
    public GameObject handR;
    // should the AI start at awake?
    public bool startAtAwake = false;

    // equipped item gameobject 
    private GameObject equippedItemGO;
    // PID controllers for each axis while aiming
    private PID xPID, yPID, zPID;
    // the time from the previous frame
    private float previousFrameTime;

    // animator component of enemy
    private Animator anim;
    // item the enemy is holding
    [HideInInspector] public ItemHolding itemHolding;

    // the enemy's LookIK
    public LookIK enemyLookIK;
    // the offset from where the player is shooting to where the enemy is positioned
    private Vector3 aimOffset;
    // the target for the AI to shoot at (probably the player)
    private GameObject target;
    // the position to cast the raycast from in order to shoot
    private Transform raycastStartSpot;
    // the current position of where the AI is aiming
    private Vector3 currentAimPosition;
    // the offset from the target's postion to where the enemy should shoot the target
    public Vector3 targetOffset;
    // the speed at which the enemy rotates
    public float rotationSpeed = 10; 

    /// </summary> Set up this GOAP agent and the PID controller, init vars </summary>
    public override void Awake()
    {
        base.Awake();

        goals.Add(GoapGoal.Goals.KILL_PLAYER, new KillPlayerGoal(GoapGoal.Goals.KILL_PLAYER, 1));

        dataSet.SetData(GoapAction.Effects.PLAYER_DEAD + "0", true);
        dataSet.SetData(GoapAction.Effects.AI_DEAD + "0", false);

        possibleActions.Add(new KillAction(this, 0));

        anim = GetComponent<Animator>();

        xPID = new PID(aimP, aimI, aimD);
        yPID = new PID(aimP, aimI, aimD);
        zPID = new PID(aimP, aimI, aimD);

        if (startAtAwake) StartCoroutine(StartFiring());
    }

    /// <summary> Start the enemy's firing </summary>
    /// <returns> an ienumerator since it is a coroutine, only use the ienumerator if you need information about the progress of a coroutine </returns>
    public IEnumerator StartFiring()
    {
        yield return new WaitForEndOfFrame();
        dataSet.SetData(GoapAction.Effects.PLAYER_DEAD + "0", false);
        equippedItemGO = Instantiate(ItemPrefabsController.instance.itemPrefabs[weaponIndex], handR.transform);
        Weapon weaponScript = equippedItemGO.GetComponent<Weapon>();
        weaponScript.shootFromMiddleOfScreen = false;
        weaponScript.raycastStartSpot = equippedItemGO.transform.Find("ShootSpot");
        weaponScript.enabled = true;
        equippedItemGO.transform.localPosition = weaponScript.startPos;
        equippedItemGO.transform.localRotation = Quaternion.Euler(weaponScript.startRot);

        weaponScript.SetUser(gameObject);

        ItemType itemType = ItemType.none;
        if (equippedItemGO.GetComponent<Weapon>().type == WeaponType.Melee) itemType = ItemType.melee;
        if (equippedItemGO.GetComponent<Weapon>().type == WeaponType.Ranged) itemType = ItemType.ranged;
        itemHolding = new ItemHolding(equippedItemGO, itemType);
    }

    /// <summary> Stop firing the weapon </summary>
    public void StopFiring()
    {
        Destroy(equippedItemGO);
        itemHolding = null;
    }

    /// <summary> Adjust the aim of the enemy. This function is called automatically by Unity if the enemy's anim had IK enabled. </summary>
    void OnAnimatorIK()
    {
        if (target != null && raycastStartSpot != null)
        {
            anim.SetLookAtWeight(enemyLookIK.lookIKWeight, enemyLookIK.bodyWeight, enemyLookIK.headWeight, enemyLookIK.eyesWeight, enemyLookIK.clampWeight);

            Vector3 idealPosition = target.transform.position + targetOffset;

            aimOffset.x += xPID.Update(idealPosition.x, currentAimPosition.x, Mathf.Clamp(Time.time - previousFrameTime, 0.001f, 100));
            aimOffset.y += yPID.Update(idealPosition.y, currentAimPosition.y, Mathf.Clamp(Time.time - previousFrameTime, 0.001f, 100));
            aimOffset.z += zPID.Update(idealPosition.z, currentAimPosition.z, Mathf.Clamp(Time.time - previousFrameTime, 0.001f, 100));

            anim.SetLookAtPosition(target.transform.position + aimOffset + targetOffset);
        }
        else
        {
            target = GameObject.Find("Player_0(Clone)");

            raycastStartSpot = transform.FindDeepChild("ShootSpot");
        }

        previousFrameTime = Time.time;
    }

    /// <summary> Rotate the enemy to aim at the target </summary>
    void LateUpdate()
    {
        if (target != null && raycastStartSpot != null)
        {
            currentAimPosition = raycastStartSpot.position + raycastStartSpot.forward * Vector3.Distance(raycastStartSpot.position, target.transform.position);

            Vector3 dir = (target.transform.position - transform.position).normalized;

            Quaternion lookRot = Quaternion.LookRotation(dir);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
        }
    }

    /// <summary> Set GOAP data to display that the enemy died </summary>
    public void EnemyDead()
    {
        dataSet.SetData(GoapAction.Effects.AI_DEAD + "0", true);
    }

    /// <summary> Set GOAP data to display that the player died </summary>
    public void PlayerDead()
    {
        dataSet.SetData(GoapAction.Effects.PLAYER_DEAD + "0", true);
    }

    /// <summary> Visualize where the enemy AI is aiming </summary>
    void OnDrawGizmos()
    {
        if (target != null && raycastStartSpot != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentAimPosition, 0.5f);
        }
    }
}

/// <summary> Find a child of a gameobject with a certain name </summary>
public static class TransformDeepChildExtension
{
    /// <summary> Find a child of a gameobject with a certain name </summary>
    /// <param name="aParent"> the transform of the parent that this function should search children of </param>
    /// <param name="aName"> the name of the child this function should search for </param>
    /// <returns> the transform of the child </returns>
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == aName)
                return c;
            foreach (Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }
}
