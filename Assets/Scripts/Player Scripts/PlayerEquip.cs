using UnityEngine;

public class PlayerEquip : MonoBehaviour
{
    public int maxGrabDistance;

    private BodyParts bodyParts;
    private InventoryManager inventoryManager;
    private InputHandler inputHandler;
    private GameObject handR;

    private void Start()
    {
        bodyParts = GetComponent<BodyParts>();
        inventoryManager = GetComponent<InventoryManager>();
        inputHandler = GetComponent<InputHandler>();
        handR = bodyParts.handR;
    }

    public void Grab()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Physics.Raycast(ray, out hit, maxGrabDistance, LayerMaskController.item) && hit.collider.gameObject.GetComponent<Weapon>() != null)
        {
            GameObject weapon = hit.collider.gameObject;
            weapon.GetComponent<Rigidbody>().isKinematic = true;
            weapon.transform.SetParent(handR.transform);
            weapon.transform.localPosition = Vector3.zero;
            weapon.transform.rotation = Quaternion.identity;
            inventoryManager.AddInventoryItem(weapon, null);
            inputHandler.ChangeItemHolding(new ItemHolding(weapon, HoldingItemType.ranged));
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
