using SwordGC.AI.Actions;
using SwordGC.AI.Goap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemyAI1 : GoapAgent
{
    public int weaponIndex;
    public GameObject handR;

    private GameObject equippedItemGO;
    private ItemPrefabsController itemPrefabsController;
    private GameObject[] itemPrefabs;

    private Animator anim;
    private PlayerEquip playerEquip;
    [HideInInspector] public ItemHolding itemHolding;

    public override void Awake()
    {
        base.Awake();

        goals.Add(GoapGoal.Goals.KILL_PLAYER, new KillPlayerGoal(GoapGoal.Goals.KILL_PLAYER, 1));

        dataSet.SetData(GoapAction.Effects.PLAYER_DEAD + "0", false);

        possibleActions.Add(new KillAction(this, 0));

        //possibleActions.Add(new FindPlayerAction(this, 0));
        // possible
        // // create Actions
        // for (int i = 1; i < testPlayerCount; i++)
        // {
        //     dataSet.SetData(GoapAction.Effects.KNOCKED_OUT_PLAYER + i, false);

        //     possibleActions.Add(new KillAction(this, i));
        //     possibleActions.Add(new KnockoutPlayerAction(this, i));
        //     possibleActions.Add(new ThrowAtPlayerAction(this, i));
        // }
        // possibleActions.Add(new GrabItemAction(this));


        anim = GetComponent<Animator>();
        itemPrefabsController = FindObjectOfType<ItemPrefabsController>();
        itemPrefabs = itemPrefabsController.itemPrefabs;

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

    public LookIK enemyLookIK;
    public Vector3 offset;
    private GameObject player;

    void OnAnimatorIK()
    {
        if (player != null)
        {
            anim.SetLookAtWeight(enemyLookIK.lookIKWeight, enemyLookIK.bodyWeight, enemyLookIK.headWeight, enemyLookIK.eyesWeight, enemyLookIK.clampWeight);
            anim.SetLookAtPosition(player.transform.position + offset);
        }
        else
        {
            player = GameObject.Find("Player_0(Clone)");
        }

    }

}
