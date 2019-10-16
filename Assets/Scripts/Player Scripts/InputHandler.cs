using UnityEngine;
using Mirror;

///<summary> Used to manage all of the player input </summary>
public class InputHandler : NetworkBehaviour
{
    #region Variables
    // current item the player is holding
    private ItemHolding itemHolding;
    // the movement script of the player
    private Movement movement;
    // player equip script of player
    private PlayerEquip playerEquip;
    // inventory manager script of player
    private InventoryManager inventoryManager;

    private UIManager uIScript;
    // if player is dead
    private bool isDead;
    #endregion

    #region Initialize
    ///<summary> Initialize components </summary>
    void Start()
    {
        movement = GetComponent<Movement>();
        playerEquip = GetComponent<PlayerEquip>();
        inventoryManager = GetComponent<InventoryManager>();
        uIScript = GameObject.FindObjectOfType<UIManager>();
        itemHolding = new ItemHolding(null, ItemType.none);
    }
    #endregion

    #region DealWithInput
    ///<summary> Create the input struct and send it off to all of the main voids </summary>
    void Update()
    {
        if (!isLocalPlayer) return;

        InputStruct input = new InputStruct(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), Input.GetButton("Jump"), Input.GetButton("Sprint"), Input.GetButton("Free Rotate Camera"), Input.GetButtonDown("Pickup"), Input.GetButtonDown("Inventory"), Input.GetButton("Fire1"), Input.GetButtonUp("Fire1"), Input.GetButtonDown("Reload"), Input.GetButton("Cancel"), Input.GetButtonDown("Pause"), Input.mousePosition);

        TestMove(input);
        TestGrab(input);
        TestUI(input);
        CheckItemHolding(input);
    }

    ///<summary> Test if player should move</summary>
    private void TestMove(InputStruct input) { if (!isDead) movement.Move(input); }

    ///<summary> Test if player should grab</summary>
    private void TestGrab(InputStruct input) { if (input.pickupDown && !isDead) playerEquip.Grab(); }

    ///<summary> Test if ui should be changed </summary>
    private void TestUI(InputStruct input)
    {
        if (input.switchInventoryDown && !isDead) inventoryManager.ChangeEnabled();

        if (Input.GetKeyDown("p"))
        {
            uIScript.TogglePauseMenu();
            if (uIScript.togglePauseMenu)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    ///<summary> Check if input related to item holding should be called </summary>
    private void CheckItemHolding(InputStruct input)
    {
        if (itemHolding.type == ItemType.ranged) { itemHolding.weaponScript.CheckForUserInput(input); }

        //if (itemHolding.type == HoldingItemType.melee) { }

        //if (itemHolding.type == HoldingItemType.collectable) { }
    }

    ///<summary> Change current item that's being held </summary>
    public void ChangeItemHolding(ItemHolding itemHolding) { this.itemHolding = itemHolding; }
    #endregion

    #region SetDead
    ///<summary> Set value for is dead </summary>
    ///<param name = "dead"> value for isDead </summary>
    public void SetDead(bool dead) { isDead = dead; }
    #endregion
}

#region  InputStruct
/// <summary> Struct where input is stored </summary>
public struct InputStruct
{
    public float horAxis, vertAxis;
    public bool jump, sprint, freeRotateCamera, pickupDown, switchInventoryDown, fire1, fire1Up, reloadDown, cancel, pauseDown;
    public Vector2 mousePos;

    public InputStruct(float horAxis, float vertAxis, bool jump, bool sprint, bool freeRotateCamera, bool pickUpDown, bool switchInventoryDown, bool fire1, bool fire1Up, bool reloadDown, bool cancel, bool pauseDown, Vector2 mousePos)
    {
        this.horAxis = horAxis;
        this.vertAxis = vertAxis;
        this.jump = jump;
        this.sprint = sprint;
        this.freeRotateCamera = freeRotateCamera;
        this.pickupDown = pickUpDown;
        this.switchInventoryDown = switchInventoryDown;
        this.fire1 = fire1;
        this.fire1Up = fire1Up;
        this.reloadDown = reloadDown;
        this.cancel = cancel;
        this.pauseDown = pauseDown;
        this.mousePos = mousePos;
    }
}
#endregion
