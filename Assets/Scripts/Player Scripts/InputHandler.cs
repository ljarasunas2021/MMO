using UnityEngine;
using Mirror;

///<summary> Used to manage all of the player input </summary>
public class InputHandler : NetworkBehaviour
{
    private ItemHolding itemHolding;
    private Movement movement;
    private PlayerEquip playerEquip;
    private InventoryManager2 inventoryManager;
    private UIManager uIScript;
    private bool isDead;
    private BodyParts bodyParts;
    private SchoolBus schoolBus;
    private BattleRoyalePlayer battleRoyalePlayer;
    private Map map;
    private Storm storm;

    void Start()
    {
        movement = GetComponent<Movement>();
        playerEquip = GetComponent<PlayerEquip>();
        inventoryManager = GameObject.FindObjectOfType<InventoryManager2>();
        uIScript = GameObject.FindObjectOfType<UIManager>();
        bodyParts = GetComponent<BodyParts>();
        itemHolding = new ItemHolding(null, ItemType.none);
        UIManager.LockCursor(true);
        schoolBus = FindObjectOfType<SchoolBus>();
        battleRoyalePlayer = GetComponent<BattleRoyalePlayer>();
        storm = GameObject.FindObjectOfType<Storm>();

        if (!isLocalPlayer) return;
        map = GameObject.FindObjectOfType<Map>();
        map.player = gameObject;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        GameObject.FindObjectOfType<PlayersController>().players.Add(gameObject);
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        TestMove();
        TestGrab();
        TestUI();
        CheckItemHolding();
    }

    private void TestMove() { if (!isDead) movement.Move(); }

    private void TestGrab() { if (Input.GetButtonDown("Pickup") && !isDead) playerEquip.Grab(); }

    private void TestUI()
    {
        if (Input.GetButtonDown("Inventory") && !isDead) inventoryManager.ChangeEnabled();

        if (Input.GetButtonDown("Pause"))
        {
            uIScript.TogglePauseMenu();
            UIManager.LockCursor(!uIScript.togglePauseMenu);
        }

        if (Input.GetButtonDown("ToggleMap")) uIScript.ToggleMap();

        if (schoolBus != null && Input.GetButtonDown("SummonSchoolBus")) CheckActivateBus();
    }

    private void CheckItemHolding()
    {
        if (itemHolding.type != ItemType.none) itemHolding.weaponScript.CheckForUserInput();
    }

    private void CheckActivateBus()
    {
        if (schoolBus.activatedBus && !battleRoyalePlayer.dropped) battleRoyalePlayer.Drop();
        else if (!battleRoyalePlayer.dropped)
        {
            schoolBus.ActivateBus();
            storm.StartStorm();
        }
    }

    public void ChangeItemHolding(ItemHolding itemHolding) { this.itemHolding = itemHolding; }

    public void SetDead(bool dead) { isDead = dead; }
}
