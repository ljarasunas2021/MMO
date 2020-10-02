using UnityEngine;
using Mirror;

///<summary> Used to manage all of the player's input </summary>
public class InputHandler : NetworkBehaviour
{
    // current item holding
    private ItemHolding itemHolding;
    // components of player:
    private Movement movement;
    private PlayerEquip playerEquip;
    private BodyParts bodyParts;

    // singletons
    private InventoryManager inventoryManager;
    private UIManager uIScript;
    private Map map;

    // camera
    private Camera cam;

    // player is dead
    private bool isDead;

    /// <summary> Init vars </summary>
    void Start()
    {
        movement = GetComponent<Movement>();
        playerEquip = GetComponent<PlayerEquip>();
        bodyParts = GetComponent<BodyParts>();

        inventoryManager = InventoryManager.instance;
        uIScript = UIManager.instance;
        
        itemHolding = new ItemHolding(null, ItemType.none);
        UIManager.instance.LockCursor(true);

        cam = Camera.main;

        if (!isLocalPlayer) return;

        map = Map.instance;
        map.player = gameObject;
    }

    /// <summary> Add player to player controller </summary>
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        PlayersController.instance.players.Add(gameObject);
    }

    /// <summary> Test for input </summary>
    void Update()
    {
        if (!isLocalPlayer) return;

        TestMove();
        TestGrab();
        TestUI();
        TestInteraction();
        TestEquip();
        TestItemHolding();
    }

    /// <summary> Test for movement input </summary>
    private void TestMove() { if (!isDead) movement.Move(); }

    /// <summary> Test for grabbing input </summary>
    private void TestGrab() { if (Input.GetButtonDown("Pickup") && !isDead) playerEquip.Grab(); }

    /// <summary> Test for UI input </summary>
    private void TestUI()
    {
        if (Input.GetButtonDown("Inventory")) uIScript.ToggleInventory();

        if (Input.GetButtonDown("Pause"))
        {
            uIScript.TogglePauseMenu();
            UIManager.instance.LockCursor(!uIScript.togglePauseMenu);
        }

        if (Input.GetButtonDown("ToggleMap")) uIScript.ToggleMap();
    }

    /// <summary> Test for interaction input </summary>
    private void TestInteraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point = new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2, 0);
            Ray ray = cam.ScreenPointToRay(point);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Target target = hit.transform.gameObject.GetComponent<Target>();
                if (target != null) target.Interact();
            }
        }
    }

    /// <summary> Test for equip input </summary>
    private void TestEquip()
    {
        for (int i = 0; i < 2; i++)
        {
            if (Input.GetMouseButtonDown(i))
            {
                playerEquip.PossibleEquipSlot(i);
            }
            if (Input.GetMouseButton(i))
            {
                playerEquip.UseItem();
            }
        }
    }

    /// <summary> Test for input related to the item the player is holding </summary>
    private void TestItemHolding()
    {
        if (itemHolding.type != ItemType.none && UIManager.instance.canShoot) itemHolding.weaponScript.CheckForUserInput();
    }

    /// <summary> Change the item holding </summary>
    /// <param name="itemHolding"> the new item holding </param>
    public void ChangeItemHolding(ItemHolding itemHolding) { this.itemHolding = itemHolding; }

    /// <summary> Set the isDead variable </summary>
    /// <param name="dead"> the new value for isDead </param>
    public void SetDead(bool dead) { isDead = dead; }
}
