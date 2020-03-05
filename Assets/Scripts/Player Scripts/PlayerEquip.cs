using Mirror;
using UnityEngine;

///<summary> Allow the player to equip items </summary>
public class PlayerEquip : NetworkBehaviour
{
    public GameObject sceneObjectPrefab;
    public int weaponTimeTillDespawn = 75;

    [SyncVar(hook = nameof(ChangeItem))]
    private int equippedItem = -1;

    private GameObject equippedItemGO;
    public int maxGrabDistance;
    private BodyParts bodyParts;
    private InventoryManager2 inventoryManager;
    private InputHandler inputHandler;
    private GameObject handR;
    private Animator animator;
    private PlayerCameraManager playerCameraManager;
    private GameObject[] itemPrefabs;
    private Movement movement;
    private int hotBarIndex = -1, hotBarIndexCounter;
    private Camera mainCam;
    private bool alreadyDespawnedWeapon = false;
    private IKHandling ikHandling;

    private void Start()
    {
        animator = GetComponent<Animator>();
        bodyParts = GetComponent<BodyParts>();
        inventoryManager = GameObject.FindObjectOfType<InventoryManager2>();
        inputHandler = GetComponent<InputHandler>();
        playerCameraManager = GetComponent<PlayerCameraManager>();
        itemPrefabs = GameObject.FindObjectOfType<ItemPrefabsController>().itemPrefabs;
        mainCam = Camera.main;
        ikHandling = GetComponent<IKHandling>();

        movement = GetComponent<Movement>();
        handR = bodyParts.handR;
        inventoryManager.SetPlayer(gameObject);
    }

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
        }
    }

    [Command]
    void CmdChangeEquippedItem(int itemIndex) { equippedItem = itemIndex; }

    [Command]
    public void CmdChangeHotBarIndex(int hotBarIndex) { this.hotBarIndex = hotBarIndex; }

    private int FindIndex(GameObject item)
    {
        int index = -1;
        for (int i = 0; i < itemPrefabs.Length; i++) if (item.name.Contains(itemPrefabs[i].name)) index = i;
        return index;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && hotBarIndex != 0)
        {
            inventoryManager.EquipSlot(0);
            alreadyDespawnedWeapon = false;
        }

        if (Input.GetMouseButton(0)) hotBarIndexCounter = weaponTimeTillDespawn;

        if (Input.GetMouseButtonDown(1) && hotBarIndex != 1)
        {
            inventoryManager.EquipSlot(1);
            alreadyDespawnedWeapon = false;
        }

        if (Input.GetMouseButton(1)) hotBarIndexCounter = weaponTimeTillDespawn;

        hotBarIndexCounter--;

        if (hotBarIndexCounter < 0 && !alreadyDespawnedWeapon)
        {
            EquipItem(-1, -1);
            alreadyDespawnedWeapon = true;
        }
    }
    public void Grab()
    {
        RaycastHit hit;
        Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out hit, maxGrabDistance, 1 << LayerMaskController.item) && hit.collider.gameObject.GetComponent<Item>() != null)
        {
            GameObject item = hit.collider.gameObject;
            Destroy(item.transform.parent.gameObject);
            inventoryManager.AddInventoryItem(FindIndex(item), item.GetComponent<Item>().icon);
        }
    }

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
            if (equippedItemGO.GetComponent<Weapon>().type == WeaponType.Melee) itemType = ItemType.melee;
            if (equippedItemGO.GetComponent<Weapon>().type == WeaponType.Ranged) itemType = ItemType.ranged;
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

    [Command]
    private void CmdSetWeaponRigidBody(GameObject weapon, bool isKinematic) { weapon.transform.GetComponent<Rigidbody>().isKinematic = isKinematic; }
}

public class ItemHolding
{
    public GameObject item;
    public ItemType type;
    public Weapon weaponScript;

    public ItemHolding(GameObject item, ItemType type)
    {
        this.item = item;
        this.type = type;

        if (type == ItemType.ranged || type == ItemType.melee) weaponScript = item.GetComponent<Weapon>();
    }
}

public enum ItemType
{
    melee,
    ranged,
    collectable,
    none
}