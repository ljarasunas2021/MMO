using UnityEngine;

public class GrabItem : MonoBehaviour
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
        Debug.Log("HIIII");
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
