using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager2 : MonoBehaviour
{
    public InventoryPlaceHolder[] inventorySlots;
    public InventoryPlaceHolder[] hotBar;
    public GameObject inventoryCanvas;
    public Sprite defaultSprite;
    [HideInInspector] public bool inventoryEnabled = false;
    [HideInInspector] public GameObject player;
    private PlayerEquip playerEquip;

    void Start()
    {
        for (int i = 0; i < hotBar.Length; i++)
        {
            hotBar[i].SetHotBarIndex(i);
        }
    }

    void Update()
    {
        Debug.Log(hotBar[0].itemAndIcon.itemIndex);
    }

    public void AddInventoryItem(int itemIndex, Sprite icon)
    {
        int index = -1;
        for (int i = 0; i < inventorySlots.Length && index == -1; i++)
        {
            if (inventorySlots[i].itemAndIcon.icon == null)
            {
                inventorySlots[i].itemAndIcon = new InventoryItemAndIcon(itemIndex, icon);
                inventorySlots[i].GetComponent<Image>().sprite = icon;
                index = i;
            }
        }
    }

    public void ChangeEnabled()
    {
        inventoryCanvas.SetActive(!inventoryEnabled);

        inventoryEnabled = !inventoryEnabled;
    }

    public void EquipSlot(int slot)
    {
        Debug.Log(slot + " " + hotBar[slot].itemAndIcon.itemIndex);
        playerEquip.EquipItem(slot, hotBar[slot].itemAndIcon.itemIndex);
    }

    public void EquipItem(int slot, int itemIndex)
    {
        playerEquip.EquipItem(slot, itemIndex);
    }

    public void EndDrag(PointerEventData eventData)
    {
        foreach (InventoryPlaceHolder slot in inventorySlots) slot.CheckForDrop(eventData);
        foreach (InventoryPlaceHolder hotSlot in hotBar) hotSlot.CheckForDrop(eventData);
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
        playerEquip = player.GetComponent<PlayerEquip>();
    }
}

[System.Serializable]
///<summary> The inventory item and its icon </summary>
public class InventoryItemAndIcon
{
    public int itemIndex = -1;
    public Sprite icon;

    public InventoryItemAndIcon(int itemIndex, Sprite icon)
    {
        this.itemIndex = itemIndex;
        this.icon = icon;
    }
}
