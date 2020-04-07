using UnityEngine;
using Mirror;

///<summary> Used to manage all of the player input </summary>
public class InputHandler : NetworkBehaviour
{
    private ItemHolding itemHolding;
    private Movement movement;
    private PlayerEquip playerEquip;
    private InventoryManager inventoryManager;
    private UIManager uIScript;
    private bool isDead;
    private BodyParts bodyParts;
    private Map map;

    void Start()
    {
        movement = GetComponent<Movement>();
        playerEquip = GetComponent<PlayerEquip>();
        inventoryManager = GameObject.FindObjectOfType<InventoryManager>();
        uIScript = GameObject.FindObjectOfType<UIManager>();
        bodyParts = GetComponent<BodyParts>();
        itemHolding = new ItemHolding(null, ItemType.none);
        UIManager.LockCursor(true);

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
    }

    private void CheckItemHolding()
    {
        if (itemHolding.type != ItemType.none) itemHolding.weaponScript.CheckForUserInput();
    }

    public void ChangeItemHolding(ItemHolding itemHolding) { this.itemHolding = itemHolding; }

    public void SetDead(bool dead) { isDead = dead; }
}
