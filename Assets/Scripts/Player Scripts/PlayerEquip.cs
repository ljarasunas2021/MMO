using Mirror;
using UnityEngine;

///<summary> Allow the player to equip items </summary>
public class PlayerEquip : NetworkBehaviour
{
    // scene object prefab
    public GameObject sceneObjectPrefab;
    // time until the weapon holding can despawn if not used
    public int weaponTimeTillDespawn = 75;

    // equipped item index
    [SyncVar(hook = nameof(ChangeItem))]
    private int equippedItem = -1;

    // equipped item gameobject
    private GameObject equippedItemGO;
    // maximum grab distance
    public int maxGrabDistance;
    // scripts of player
    private BodyParts bodyParts;
    private InventoryManager inventoryManager;
    private InputHandler inputHandler;
    private GameObject handR;
    private Animator animator;
    private PlayerCameraManager playerCameraManager;
    private Movement movement;
    private IKHandling ikHandling;

    // item prefabs array
    private GameObject[] itemPrefabs;
    // hot bar vars
    private int hotBarIndex = -1, hotBarIndexCounter;
    // maincamera
    private Camera mainCam;
    // weapon has been despawned bool
    private bool alreadyDespawnedWeapon = false;    

    // init vars
    private void Start()
    {
        animator = GetComponent<Animator>();
        bodyParts = GetComponent<BodyParts>();
        inventoryManager = GameObject.FindObjectOfType<InventoryManager>();
        inputHandler = GetComponent<InputHandler>();
        playerCameraManager = GetComponent<PlayerCameraManager>();
        itemPrefabs = ItemPrefabsController.instance.itemPrefabs;
        mainCam = Camera.main;
        ikHandling = GetComponent<IKHandling>();

        movement = GetComponent<Movement>();
        handR = bodyParts.handR;

        hotBarIndexCounter = weaponTimeTillDespawn;

        if (!isLocalPlayer) return;

        inventoryManager.SetPlayer(gameObject);
    }

    // change the current item
    private void ChangeItem(int itemIndex)
    {
        foreach (Transform weapon in handR.transform) Destroy(weapon.gameObject);
        //while (handR.transform.childCount > 0) DestroyImmediate(handR.transform.GetChild(0).gameObject);

        if (itemIndex != -1)
        {
            equippedItemGO = Instantiate(itemPrefabs[itemIndex], handR.transform);
            Weapon weaponScript = equippedItemGO.GetComponent<Weapon>();
            equippedItemGO.transform.localPosition = weaponScript.startPos;
            equippedItemGO.transform.localRotation = Quaternion.Euler(weaponScript.startRot);
            weaponScript.used = true;
        }
    }

    // change equipped item index
    [Command]
    void CmdChangeEquippedItem(int itemIndex)
    {
        equippedItem = itemIndex;
    }

    // change hot bar index
    [Command]
    public void CmdChangeHotBarIndex(int hotBarIndex) { this.hotBarIndex = hotBarIndex; }

    // find correct item index
    private int FindIndex(GameObject item)
    {
        int index = -1;
        for (int i = 0; i < itemPrefabs.Length; i++) if (item.name.Contains(itemPrefabs[i].name)) index = i;
        return index;
    }

    // possibly equip a new slot
    public void PossibleEquipSlot(int index)
    {
        if (hotBarIndex != index)
        {
            inventoryManager.EquipSlot(index);
            alreadyDespawnedWeapon = false;
        }
    }

    // reset hotbarIndex counter if use item
    public void UseItem(int index)
    {
        hotBarIndexCounter = weaponTimeTillDespawn;
    }

    // control hot bar index counter
    private void Update()
    {
        hotBarIndexCounter--;

        if (hotBarIndexCounter < 0 && !alreadyDespawnedWeapon)
        {
            EquipItem(-1, -1);
            alreadyDespawnedWeapon = true;
        }
    }

    // grab weapon
    public void Grab()
    {
        RaycastHit hit;
        Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out hit, maxGrabDistance, 1 << LayerMaskController.item)) {
            Item item = hit.collider.gameObject.GetComponent<Item>();
            if (item != null)
            {
                inventoryManager.AddInventoryItem(FindIndex(item.gameObject), item.icon);
                Destroy(item.transform.parent.gameObject);
            }
        }
    }

    // equip item
    public void EquipItem(int hotBarIndex, int itemIndex)
    {
        CmdChangeHotBarIndex(hotBarIndex);
        CmdChangeEquippedItem(itemIndex);
        EnableWeaponScript(itemIndex);
    }

    // enable the weapon script of the newly equipped item
    private void EnableWeaponScript(int itemIndex)
    {
        int upperBodyState = 0;
        CameraModes cameraMode = 0;

        if (itemIndex == -1)
        {
            cameraMode = CameraModes.cinematic;
            upperBodyState = (int)UpperBodyStates.none;
        }
        else
        {
            Weapon weapon = equippedItemGO.GetComponent<Weapon>();
            weapon.enabled = true;
            weapon.SetUser(gameObject);
            weapon.SetHotBarIndex(hotBarIndex);

            ItemType itemType = ItemType.none;
            if (weapon.type == WeaponType.Melee) itemType = ItemType.melee;
            if (weapon.type == WeaponType.Ranged) itemType = ItemType.ranged;
            inputHandler.ChangeItemHolding(new ItemHolding(equippedItemGO, itemType));

            if (weapon.type == WeaponType.Ranged)
            {
                if (weapon.rangedHold == RangedHoldType.pistol) upperBodyState = (int)UpperBodyStates.pistolHold;
                else if (weapon.rangedHold == RangedHoldType.shotgun) upperBodyState = (int)UpperBodyStates.shotgunHold;

                cameraMode = CameraModes.locked;

                ikHandling.SwitchLookIK(LookIKTypes.Basic);
            }
            else
            {
                upperBodyState = (int)UpperBodyStates.swordHold;

                cameraMode = CameraModes.closeUp;

                ikHandling.SwitchLookIK(LookIKTypes.Weapon);
            }
        }

        animator.SetInteger(Parameters.upperBodyState, upperBodyState);
        playerCameraManager.ChangeCam(cameraMode);
    }

    // use rigidbody on server
    [Command]
    private void CmdSetWeaponRigidBody(GameObject weapon, bool isKinematic) { weapon.transform.GetComponent<Rigidbody>().isKinematic = isKinematic; }
}

// current item held
public class ItemHolding
{
    // actual item
    public GameObject item;
    // item type
    public ItemType type;
    // item's weapon script
    public Weapon weaponScript;

    // constructor
    public ItemHolding(GameObject item, ItemType type)
    {
        this.item = item;
        this.type = type;

        if (type == ItemType.ranged || type == ItemType.melee) weaponScript = item.GetComponent<Weapon>();
    }
}

// types of items
public enum ItemType
{
    melee,
    ranged,
    collectable,
    none
}