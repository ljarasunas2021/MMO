using UnityEngine;
using System.Collections;

public class PlayerEquip : MonoBehaviour
{
    public int maxGrabDistance;

    private BodyParts bodyParts;
    private InventoryManager inventoryManager;
    private InputHandler inputHandler;
    private GameObject handR;
    private Animator animator;
    private PlayerCameraManager playerCameraManager;

    private void Start()
    {
        animator = GetComponent<Animator>();
        bodyParts = GetComponent<BodyParts>();
        inventoryManager = GetComponent<InventoryManager>();
        inputHandler = GetComponent<InputHandler>();
        playerCameraManager = GetComponent<PlayerCameraManager>();
        handR = bodyParts.handR;
    }

    public void Grab()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out hit, maxGrabDistance, 1 << LayerMaskController.item) && hit.collider.gameObject.GetComponent<Weapon>() != null)
        {
            GameObject weapon = hit.collider.gameObject;
            weapon.GetComponent<Rigidbody>().isKinematic = true;
            weapon.transform.SetParent(handR.transform);
            weapon.transform.localPosition = weapon.GetComponent<Weapon>().startPos;
            weapon.transform.localRotation = Quaternion.Euler(weapon.GetComponent<Weapon>().startRot);
            inventoryManager.AddInventoryItem(weapon, null);
            inputHandler.ChangeItemHolding(new ItemHolding(weapon, HoldingItemType.ranged));
            animator.SetInteger(Parameters.upperBodyState, 2);
            playerCameraManager.ChangeCam(CameraModes.locked);
        }
    }
}

public class ItemHolding
{
    public GameObject item;
    public HoldingItemType type;
    public Weapon weaponScript;

    public ItemHolding(GameObject item, HoldingItemType type)
    {
        this.item = item;
        this.type = type;

        if (type == HoldingItemType.ranged) weaponScript = item.GetComponent<Weapon>();
        else weaponScript = null;
    }
}

public enum HoldingItemType
{
    melee,
    ranged,
    collectable,
    none
}
