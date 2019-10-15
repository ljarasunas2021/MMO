using Mirror;
using UnityEngine;

///<summary> Allow the player to equip items </summary>
public class PlayerEquip : NetworkBehaviour
{
    #region initialize
    // scene object
    public GameObject sceneObjectPrefab;
    [SyncVar(hook = nameof(ChangeItem))]
    // equipped item index
    private int equippedItem = -1;
    // equipped item gameObject
    private GameObject equippedItemGO;
    // max grab distance
    public int maxGrabDistance;
    // body parts script
    private BodyParts bodyParts;
    // inventory manager script
    private InventoryManager inventoryManager;
    // input handler script
    private InputHandler inputHandler;
    // handR gameobject
    private GameObject handR;
    // animator attached to player
    private Animator animator;
    // player camera manager script
    private PlayerCameraManager playerCameraManager;
    // array of item prefabs
    private GameObject[] itemPrefabs;
    #endregion

    #region Initialize
    ///<summary> Initialize components</summary>
    private void Start()
    {
        animator = GetComponent<Animator>();
        bodyParts = GetComponent<BodyParts>();
        inventoryManager = GetComponent<InventoryManager>();
        inputHandler = GetComponent<InputHandler>();
        playerCameraManager = GetComponent<PlayerCameraManager>();
        itemPrefabs = GameObject.FindObjectOfType<ItemPrefabsController>().itemPrefabs;
        handR = bodyParts.handR;
    }
    #endregion

    #region ChangingItemBehaviour
    ///<summary> Make it so that the current item is visible based on that item's index </summary>
    ///<param name = "itemIndex"> Index of item that will be made visible </param>
    private void ChangeItem(int itemIndex)
    {
        //while (handR.transform.childCount > 0) DestroyImmediate(handR.transform.GetChild(0).gameObject);

        if (itemIndex != -1)
        {
            equippedItemGO = Instantiate(itemPrefabs[itemIndex], handR.transform);
            Weapon weaponScript = equippedItemGO.GetComponent<Weapon>();
            equippedItemGO.transform.localPosition = weaponScript.startPos;
            equippedItemGO.transform.localRotation = Quaternion.Euler(weaponScript.startRot);
        }
    }

    [Command]
    ///<summary> Change the equipped item </summary>
    ///<param name = "selectedItem"> Item to make the index of </param>
    void CmdChangeEquippedItem(GameObject selectedItem) { equippedItem = FindIndex(selectedItem); }

    ///<summary> Find the index of the gameObject in the prefab array </summary>
    ///<param name = "item"> item to get the index of </param>
    private int FindIndex(GameObject item)
    {
        int index = -1;
        for (int i = 0; i < itemPrefabs.Length; i++) if (item.name.Contains(itemPrefabs[i].name)) index = i;
        return index;
    }
    #endregion

    #region Grab
    ///<summary> Attempt a grab <summary>
    public void Grab()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out hit, maxGrabDistance, 1 << LayerMaskController.item) && hit.collider.gameObject.GetComponent<Weapon>() != null)
        {
            GameObject item = hit.collider.gameObject;
            CmdChangeEquippedItem(item);
            Destroy(item.transform.parent.gameObject);
            EnableWeaponScript();
            inventoryManager.AddInventoryItem(equippedItemGO, null);
        }
    }

    private void EnableWeaponScript()
    {
        Weapon weapon = equippedItemGO.GetComponent<Weapon>();
        weapon.enabled = true;
        weapon.SetUser(gameObject);
        inputHandler.ChangeItemHolding(new ItemHolding(equippedItemGO, ItemType.ranged));

        int upperBodyState = 0;
        CameraModes cameraMode = 0;

        if (weapon.type == WeaponType.Ranged)
        {
            if (weapon.rangedHold == RangedHoldType.pistol) upperBodyState = (int)UpperBodyStates.pistolHold;
            else if (weapon.rangedHold == RangedHoldType.shotgun) upperBodyState = (int)UpperBodyStates.shotgunHold;

            cameraMode = CameraModes.locked;
        }
        else
        {
            upperBodyState = (int)UpperBodyStates.swordHold;

            cameraMode = CameraModes.closeUp;
        }

        animator.SetInteger(Parameters.upperBodyState, upperBodyState);

        playerCameraManager.ChangeCam(cameraMode);
    }

    [Command]
    ///<summary> Used to set rigidbody </summary>
    ///<param name = "weapon"> rigidbody's gameObject </param>
    ///<param name = "isKinematic"> value to set is kinematic to </param>
    private void CmdSetWeaponRigidBody(GameObject weapon, bool isKinematic) { weapon.transform.GetComponent<Rigidbody>().isKinematic = isKinematic; }
    #endregion
}

#region ItemHolding
///<summary> Holds current item that is being held </summary>
public class ItemHolding
{
    public GameObject item;
    public ItemType type;
    public Weapon weaponScript;

    public ItemHolding(GameObject item, ItemType type)
    {
        this.item = item;
        this.type = type;

        if (type == ItemType.ranged) weaponScript = item.GetComponent<Weapon>();
        else weaponScript = null;
    }
}
#endregion

#region ItemType
/// <summary> type of item </summary>
public enum ItemType
{
    melee,
    ranged,
    collectable,
    none
}
#endregion
