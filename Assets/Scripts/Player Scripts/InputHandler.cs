using UnityEngine;
using Mirror;

///<summary> Used to manage all of the player input </summary>
public class InputHandler : NetworkBehaviour
{
    // current item holding
    private ItemHolding itemHolding;
    // appropriate scripts:
    private Movement movement;
    private PlayerEquip playerEquip;
    private InventoryManager inventoryManager;
    private UIManager uIScript;
    private BodyParts bodyParts;
    private Map map;

    // camera
    private Camera cam;

    // player is dead
    private bool isDead;

    // init vars
    void Start()
    {
        movement = GetComponent<Movement>();
        playerEquip = GetComponent<PlayerEquip>();
        bodyParts = GetComponent<BodyParts>();

        inventoryManager = InventoryManager.instance;
        uIScript = UIManager.instance;
        
        itemHolding = new ItemHolding(null, ItemType.none);
        UIManager.LockCursor(true);

        cam = Camera.main;

        if (!isLocalPlayer) return;

        map = Map.instance;
        map.player = gameObject;
    }

    // add player to players controller
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        PlayersController.instance.players.Add(gameObject);
    }

    // test for ui
    void Update()
    {
        if (!isLocalPlayer) return;

        TestMove();
        TestGrab();
        TestUI();
        TestInteraction();
        TestEquip();
        CheckItemHolding();
    }

    // test for movement
    private void TestMove() { if (!isDead) movement.Move(); }

    // test for grabbing
    private void TestGrab() { if (Input.GetButtonDown("Pickup") && !isDead) playerEquip.Grab(); }

    // test for UI
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

    // test interaction
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
                playerEquip.UseItem(i);
            }
        }
    }

    // test for input related to items holding
    private void CheckItemHolding()
    {
        if (itemHolding.type != ItemType.none && UIManager.canShoot) itemHolding.weaponScript.CheckForUserInput();
    }

    // change the item holding
    public void ChangeItemHolding(ItemHolding itemHolding) { this.itemHolding = itemHolding; }

    // change is dead
    public void SetDead(bool dead) { isDead = dead; }
}
