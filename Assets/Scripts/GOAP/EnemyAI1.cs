using System.Collections;
using System.Collections.Generic;
using Mirror;
using SwordGC.AI.Actions;
using SwordGC.AI.Goap;
using UnityEngine;

// GOAP AI system for enemy 1
public class EnemyAI1 : GoapAgent
{
    // current index of weapon holding
    public int weaponIndex;
    // PID controller for aim
    public float aimP, aimI, aimD;
    // right hand
    public GameObject handR;
    // start AI at awake?
    public bool startAtAwake = false;

    // equipped item gameobject 
    private GameObject equippedItemGO;
    // PID controllers for each axis
    private PID xPID, yPID, zPID;
    // the time from the previous frame
    private float currentTime;

    // animator
    private Animator anim;
    // player equip component
    private PlayerEquip playerEquip;
    // object being held
    [HideInInspector] public ItemHolding itemHolding;

    // set up goap agent and PID controller
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

        offset1 = offset;

        if (startAtAwake) StartFiring();
    }

    // start firing weapon
    public void StartFiring()
    {
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

    // stop firign weapon
    public void StopFiring()
    {
        Destroy(equippedItemGO);
        itemHolding = null;
    }

    // when enemy should look
    public LookIK enemyLookIK;
    // offset for shooting
    public Vector3 offset;
    // target = player
    private GameObject player;
    // Starting position to cast raycast to shoot
    private Transform raycastStartSpot;
    // current position and offset
    private Vector3 currentPosition, offset1;

    // Aim
    void OnAnimatorIK()
    {
        if (player != null && raycastStartSpot != null)
        {
            anim.SetLookAtWeight(enemyLookIK.lookIKWeight, enemyLookIK.bodyWeight, enemyLookIK.headWeight, enemyLookIK.eyesWeight, enemyLookIK.clampWeight);

            Vector3 idealPosition = player.transform.position + offset1;

            offset.x += xPID.Update(idealPosition.x, currentPosition.x, Mathf.Clamp(Time.time - currentTime, 0.001f, 100));
            offset.y += yPID.Update(idealPosition.y, currentPosition.y, Mathf.Clamp(Time.time - currentTime, 0.001f, 100));
            offset.z += zPID.Update(idealPosition.z, currentPosition.z, Mathf.Clamp(Time.time - currentTime, 0.001f, 100));

            anim.SetLookAtPosition(player.transform.position + offset + offset1);
        }
        else
        {
            player = GameObject.Find("Player_0(Clone)");

            raycastStartSpot = transform.FindDeepChild("ShootSpot");
        }

        currentTime = Time.time;
    }

    // rotate enemy
    void LateUpdate()
    {
        if (player != null && raycastStartSpot != null)
        {
            currentPosition = raycastStartSpot.position + raycastStartSpot.forward * Vector3.Distance(raycastStartSpot.position, player.transform.position);

            Vector3 dir = (player.transform.position - transform.position).normalized;

            Quaternion lookRot = Quaternion.LookRotation(dir);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10);
        }

        xPID.pFactor = aimP;
        xPID.iFactor = aimI;
        xPID.dFactor = aimD;
        yPID.pFactor = aimP;
        yPID.iFactor = aimI;
        yPID.dFactor = aimD;
        zPID.pFactor = aimP;
        zPID.iFactor = aimI;
        zPID.dFactor = aimD;
    }

    // set data that this enemy is dead
    public void Dead()
    {
        dataSet.SetData(GoapAction.Effects.AI_DEAD + "0", true);
    }

    // set data that this player is dead
    public void PlayerDead()
    {
        dataSet.SetData(GoapAction.Effects.PLAYER_DEAD + "0", true);
    }

    // visualize shooting
    void OnDrawGizmos()
    {
        if (player != null && raycastStartSpot != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentPosition, 0.5f);
        }
    }
}

// find gameobject with name aName
public static class TransformDeepChildExtension
{
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
