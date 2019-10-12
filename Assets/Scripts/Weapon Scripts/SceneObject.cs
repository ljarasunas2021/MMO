using UnityEngine;
using Mirror;

public class SceneObject : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnChangeEquipment))]
    public GameObject equippedItem;

    public GameObject ballPrefab;
    public GameObject boxPrefab;
    public GameObject cylinderPrefab;

    void OnChangeEquipment(GameObject newEquippedItem)
    {
        while (transform.childCount > 0) DestroyImmediate(transform.GetChild(0).gameObject);

        // Use the new value, not the SyncVar property value
        SetEquippedItem(newEquippedItem);
    }

    // SetEquippedItem is called on the client from OnChangeEquipment (above),
    // and on the server from CmdDropItem in the PlayerEquip script.
    public void SetEquippedItem(GameObject newEquippedItem)
    {
        newEquippedItem.transform.SetParent(transform);
        newEquippedItem.transform.localPosition = Vector3.zero;
        newEquippedItem.transform.localEulerAngles = Vector3.zero;
    }
}