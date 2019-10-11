using UnityEngine;

///<summary> Allow the player to equip items </summary>
public class PlayerEquip : MonoBehaviour
{
    #region initialize
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
        handR = bodyParts.handR;
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
            GameObject weapon = hit.collider.gameObject;
            weapon.GetComponent<Rigidbody>().isKinematic = true;
            weapon.transform.SetParent(handR.transform);
            Weapon weaponScript = weapon.GetComponent<Weapon>();
            weaponScript.enabled = true;
            weapon.transform.localPosition = weaponScript.startPos;
            weapon.transform.localRotation = Quaternion.Euler(weaponScript.startRot);
            inventoryManager.AddInventoryItem(weapon, null);
            inputHandler.ChangeItemHolding(new ItemHolding(weapon, ItemType.ranged));
            animator.SetInteger(Parameters.upperBodyState, 2);
            playerCameraManager.ChangeCam(CameraModes.locked);
        }
    }
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
