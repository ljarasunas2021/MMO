using System.Collections;
using System.Collections.Generic;
using Mirror;
using SwordGC.AI.Actions;
using SwordGC.AI.Goap;
using UnityEngine;

public class EnemyAI1 : GoapAgent
{
    public int weaponIndex;
    public float aimP, aimI, aimD;
    public GameObject handR;
    public bool startAtAwake = false;

    private GameObject equippedItemGO;
    private ItemPrefabsController itemPrefabsController;
    private GameObject[] itemPrefabs;
    private PID xPID, yPID, zPID;
    private float currentTime;

    private Animator anim;
    private PlayerEquip playerEquip;
    [HideInInspector] public ItemHolding itemHolding;

    public override void Awake()
    {
        base.Awake();

        goals.Add(GoapGoal.Goals.KILL_PLAYER, new KillPlayerGoal(GoapGoal.Goals.KILL_PLAYER, 1));

        dataSet.SetData(GoapAction.Effects.PLAYER_DEAD + "0", true);
        dataSet.SetData(GoapAction.Effects.AI_DEAD + "0", false);

        possibleActions.Add(new KillAction(this, 0));

        anim = GetComponent<Animator>();
        itemPrefabsController = FindObjectOfType<ItemPrefabsController>();
        itemPrefabs = itemPrefabsController.itemPrefabs;

        xPID = new PID(aimP, aimI, aimD);
        yPID = new PID(aimP, aimI, aimD);
        zPID = new PID(aimP, aimI, aimD);

        offset1 = offset;

        if (startAtAwake) StartFiring();
    }

    public void StartFiring()
    {
        dataSet.SetData(GoapAction.Effects.PLAYER_DEAD + "0", false);
        equippedItemGO = Instantiate(itemPrefabs[weaponIndex], handR.transform);
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

    public void StopFiring()
    {
        Destroy(equippedItemGO);
        itemHolding = null;
    }

    public LookIK enemyLookIK;
    public Vector3 offset;
    private GameObject player;
    private Transform raycastStartSpot;
    private Vector3 currentPosition, offset1;

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

    public void Dead()
    {
        dataSet.SetData(GoapAction.Effects.AI_DEAD + "0", true);
    }

    public void PlayerDead()
    {
        dataSet.SetData(GoapAction.Effects.PLAYER_DEAD + "0", true);
    }

    void OnDrawGizmos()
    {
        if (player != null && raycastStartSpot != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(currentPosition, 0.5f);
        }
    }
}

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
