using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // singleton
    public static InventoryManager instance;
    // inventory slots array
    public InventoryPlaceHolder[] inventorySlots;
    // array of hot bar slots
    public InventoryPlaceHolder[] hotBar;
    // canvas for the inventory
    public GameObject inventoryCanvas;
    // blank sprite for if slot is empty
    public Sprite defaultSprite;
    // if the inventory is shown or not
    [HideInInspector] public bool inventoryEnabled = false;
    // the local player
    [HideInInspector] public GameObject player;
    // the player equip script of the player
    private PlayerEquip playerEquip;

    // set singleton, set default indexes and values for arrays
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Debug.LogError("There already exists an instance of the inventory script");
        }

        for (int i = 0; i < hotBar.Length; i++)
        {
            hotBar[i].SetHotBarIndex(i);
        }
        foreach (InventoryPlaceHolder inventorySlot in inventorySlots) inventorySlot.itemAndIcon = new InventoryItemAndIcon(-1, null);
    }

    // Add an inventory item to the inventory
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

    // toggle the inventory on and off
    public void ChangeEnabled()
    {
        inventoryCanvas.SetActive(!inventoryEnabled);

        inventoryEnabled = !inventoryEnabled;

        UIManager.LockCursor(!inventoryEnabled);

        playerEquip.CmdChangeHotBarIndex(-1);
    }

    // equip a certain hot bar slot (item index isn't known)
    public void EquipSlot(int slot)
    {
        playerEquip.EquipItem(slot, hotBar[slot].itemAndIcon.itemIndex);
    }

    // finish dragging an inventory placeholder, switch values and graphics
    public void EndDrag(PointerEventData eventData)
    {
        foreach (InventoryPlaceHolder slot in inventorySlots) slot.CheckForDrop(eventData);
        foreach (InventoryPlaceHolder hotSlot in hotBar) hotSlot.CheckForDrop(eventData);
    }

    // set the player variables
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
    // index of the item in the item array
    public int itemIndex = -1;
    // icon that is shown in the inventory
    public Sprite icon;

    // create a new inventory item and icon
    public InventoryItemAndIcon(int itemIndex, Sprite icon)
    {
        this.itemIndex = itemIndex;
        this.icon = icon;
    }
}
