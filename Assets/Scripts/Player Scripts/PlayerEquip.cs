using Mirror;
using UnityEngine;
using MMO.UI.Inventory;

namespace MMO.Player
{
    ///<summary> Allow the player to equip and unequip items </summary>
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
        // main camera
        private Camera mainCam;
        // weapon has been despawned bool
        private bool alreadyDespawnedWeapon = false;    

        /// <summary> Init vars </summary>
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

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            PickupItem item = hit.gameObject.GetComponent<PickupItem>();
            if (item != null && item.type == pickupItemType.Coin)
            {
                Destroy(hit.gameObject);
                Money.instance.IncreaseMoney(1);
            }
        }

        /// <summary> Change the current equipped item </summary>
        /// <param name="itemIndex"> the index of the new equipped item </param>        
        private void ChangeItem(int itemIndex)
        {
            foreach (Transform handChild in handR.transform)
            {
                if (!handChild.name.Contains("mixamorig"))
                {
                    // child of hand isn't finger, meaning it is a weapon
                    Destroy(handChild.gameObject);
                }
            }
            //while (handR.transform.childCount > 0) DestroyImmediate(handR.transform.GetChild(0).gameObject);

            if (itemIndex != -1)
            {
                equippedItemGO = Instantiate(itemPrefabs[itemIndex], handR.transform);
                Weapon weaponScript = equippedItemGO.GetComponent<Weapon>();
                Collectable collectableScript = equippedItemGO.GetComponent<Collectable>();
                if (weaponScript != null)
                {
                    equippedItemGO.transform.localPosition = weaponScript.startPos;
                    equippedItemGO.transform.localRotation = Quaternion.Euler(weaponScript.startRot);
                }

                if (weaponScript != null)
                    weaponScript.used = true;
            }
        }

        /// <summary> Change the equipped item index </summary>
        /// <param name="itemIndex"> new equipped item index </param>
        [Command]
        void CmdChangeEquippedItem(int itemIndex)
        {
            equippedItem = itemIndex;
        }

        /// <summary> Change the hot bar index </summary>
        /// <param name="hotBarIndex"> new hot bar index </param>
        [Command]
        public void CmdChangeHotBarIndex(int hotBarIndex) { this.hotBarIndex = hotBarIndex; }

        /// <summary> Find the item index of an item </summary>
        /// <param name="item"> the item </param>
        /// <returns> the item index of the item </returns>
        private int FindItemIndex(GameObject item)
        {
            int index = -1;
            for (int i = 0; i < itemPrefabs.Length; i++) if (item.name.Contains(itemPrefabs[i].name)) index = i;
            return index;
        }

        /// <summary> Equip new slot (possibly) </summary>
        /// <param name="index"> slot index </param>
        public void PossibleEquipSlot(int index)
        {
            if (hotBarIndex != index)
            {
                inventoryManager.EquipSlot(index);
                alreadyDespawnedWeapon = false;
            }
        }

        /// <summary> Reset hotbarIndex counter if use item </summary>
        public void UseItem()
        {
            hotBarIndexCounter = weaponTimeTillDespawn;
        }

        /// <summary> Control hotbar index counter </summary>
        private void Update()
        {
            hotBarIndexCounter--;

            if (hotBarIndexCounter < 0 && !alreadyDespawnedWeapon)
            {
                EquipItem(-1, -1);
                alreadyDespawnedWeapon = true;
            }
        }

        /// <summary> Grab an item </summary>
        public void Grab()
        {
            RaycastHit hit;
            Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            if (Physics.Raycast(ray, out hit, maxGrabDistance, 1 << LayerMaskController.item))
            {
                Item item = hit.collider.gameObject.GetComponent<Item>();
                if (item != null)
                {
                    if (item.GetComponent<Weapon>() != null)
                    {
                        inventoryManager.AddInventoryItem(FindItemIndex(item.gameObject), item.icon);
                    }
                    else
                    {
                        PotionTypes potionType = item.gameObject.GetComponent<Collectable>().potionType;

                        if (potionType == PotionTypes.Health)
                        {
                            PotionUI.instance.IncreaseHealthPotionText(1);
                        }
                    }

                    Destroy(item.transform.parent.gameObject);
                }
            }
        }

        /// <summary> Equip an item </summary>
        /// <param name="hotBarIndex"> the hot bar index of the new item </param>
        /// <param name="itemIndex"> the new item's index </param>
        public void EquipItem(int hotBarIndex, int itemIndex)
        {
            CmdChangeHotBarIndex(hotBarIndex);
            CmdChangeEquippedItem(itemIndex);
            EnableWeaponScript(itemIndex);
        }


        /// <summary> Enable the weapon script on a newly equipped weapon </summary>
        /// <param name="itemIndex"> item index of newly equipped item </param>
        private void EnableWeaponScript(int itemIndex)
        {
            int upperBodyState = 0;
            CameraModes cameraMode = 0;

            if (itemIndex == -1)
            {
                cameraMode = CameraModes.cinematic;
                upperBodyState = (int)PlayerAnimUpperBodyState.none;
            }
            else
            {
                Weapon weapon = equippedItemGO.GetComponent<Weapon>();
                if (weapon != null)
                {
                    weapon.enabled = true;
                    weapon.SetUser(gameObject);
                    weapon.SetHotBarIndex(hotBarIndex);
                }

                ItemType itemType = ItemType.none;
                if (weapon != null)
                {
                    if (weapon.type == WeaponType.Melee) itemType = ItemType.melee;
                    else if (weapon.type == WeaponType.Ranged) itemType = ItemType.ranged;
                }
                else
                {
                    itemType = ItemType.collectable;
                }

                inputHandler.ChangeItemHolding(new ItemHolding(equippedItemGO, itemType));

                if (weapon != null)
                {
                    if (weapon.type == WeaponType.Ranged)
                    {
                        if (weapon.rangedHold == RangedHoldType.pistol) upperBodyState = (int)PlayerAnimUpperBodyState.pistolHold;
                        else if (weapon.rangedHold == RangedHoldType.shotgun) upperBodyState = (int)PlayerAnimUpperBodyState.shotgunHold;

                        cameraMode = CameraModes.locked;

                        ikHandling.SwitchLookIK(LookIKTypes.Basic);
                    }
                    else
                    {
                        upperBodyState = (int)PlayerAnimUpperBodyState.swordHold;

                        cameraMode = CameraModes.closeUp;

                        ikHandling.SwitchLookIK(LookIKTypes.Weapon);
                    }
                }
            }

            animator.SetInteger(PlayerAnimParameters.upperBodyState, upperBodyState);
            playerCameraManager.ChangeCam(cameraMode);
        }

        /// <summary> Set a rigidbody's isKinematic var on the server </summary>
        /// <param name="weapon"> weapon gameobject </param>
        /// <param name="isKinematic"> should the weapon be kinematic </param>
        [Command]
        private void CmdSetWeaponRigidBody(GameObject weapon, bool isKinematic) { weapon.transform.GetComponent<Rigidbody>().isKinematic = isKinematic; }
    }

    /// <summary> Holds vars for item thats being held </summary>
    public class ItemHolding
    {
        // actual item
        public GameObject item;
        // item type
        public ItemType type;
        // item's weapon script
        public Weapon weaponScript;

        /// <summary> Constructor</summary>
        /// <param name="item"> actual item gameobject </param>
        /// <param name="type"> item's type </param>
        public ItemHolding(GameObject item, ItemType type)
        {
            this.item = item;
            this.type = type;

            if (type == ItemType.ranged || type == ItemType.melee) weaponScript = item.GetComponent<Weapon>();
        }
    }

    /// <summary> Type of items </summary>
    public enum ItemType
    {
        melee,
        ranged,
        collectable,
        none
    }
}
