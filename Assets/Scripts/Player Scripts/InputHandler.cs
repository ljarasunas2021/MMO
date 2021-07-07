using UnityEngine;
using Mirror;

///<summary> Used to manage all of the player input </summary>
public class InputHandler : NetworkBehaviour
{
    #region Variables
    public GameObject nonRagdoll;
    // current item the player is holding
    private ItemHolding itemHolding;
    // the movement script of the player
    private Movement movement;
    // player equip script of player
    private PlayerEquip playerEquip;
    // inventory manager script of player
    private InventoryManager2 inventoryManager;

    private UIManager uIScript;
    // if player is dead
    private bool isDead;

    private BodyParts bodyParts;
    #endregion

    #region Initialize
    ///<summary> Initialize components </summary>
    void Start()
    {
        movement = nonRagdoll.GetComponent<Movement>();
        playerEquip = GetComponent<PlayerEquip>();
        inventoryManager = GameObject.FindObjectOfType<InventoryManager2>();
        uIScript = GameObject.FindObjectOfType<UIManager>();
        bodyParts = GetComponent<BodyParts>();
        itemHolding = new ItemHolding(null, ItemType.none);
    }
    #endregion

    #region DealWithInput
    ///<summary> Create the input struct and send it off to all of the main voids </summary>
    void Update()
    {
        if (!isLocalPlayer) return;

        TestMove();
        TestGrab();
        TestUI();
        CheckItemHolding();
    }

    ///<summary> Test if player should move</summary>
    private void TestMove() { if (!isDead) movement.Move(); }

    ///<summary> Test if player should grab</summary>
    private void TestGrab() { if (Input.GetButtonDown("Pickup") && !isDead) playerEquip.Grab(); }

    ///<summary> Test if ui should be changed </summary>
    private void TestUI()
    {
        if (Input.GetButtonDown("Inventory") && !isDead) inventoryManager.ChangeEnabled();

        if (Input.GetButtonDown("Pause"))
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

        if (Input.GetButtonDown("Dialogue Skip") && uIScript.toggleDialogueBox)
        {
            uIScript.PlayDialogue();
        }
    }

    ///<summary> Check if input related to item holding should be called </summary>
    private void CheckItemHolding()
    {
        if (itemHolding.type == ItemType.ranged) { itemHolding.weaponScript.CheckForUserInput(); }

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
