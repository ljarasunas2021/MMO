using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary> Manages the inventory </summary>
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

    /// <summary> Create singleton, set default indexes and values for arrays</summary>
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

    /// <summary> Add an inventory item </summary>
    /// <param name="itemIndex"> index from item prefabs for an item to place in inventory </param>
    /// <param name="icon"> icon to put in inventory panel </param>
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

    /// <summary> Toggle the inventory on and off </summary>
    public void ChangeEnabled()
    {
        inventoryCanvas.SetActive(!inventoryEnabled);

        inventoryEnabled = !inventoryEnabled;

        UIManager.instance.LockCursor(!inventoryEnabled);

        playerEquip.CmdChangeHotBarIndex(-1);
    }

    /// <summary> equip a certain hot bar slot </summary>
    /// <param name="slot"> the slot to equip </param>
    public void EquipSlot(int slot)
    {
        playerEquip.EquipItem(slot, hotBar[slot].itemAndIcon.itemIndex);
    }

    /// <summary> Finish dragging an inventory placeholder, switch values and graphics </summary>
    /// <param name="eventData"> informaiton about the drag </param>
    public void EndDrag(PointerEventData eventData)
    {
        foreach (InventoryPlaceHolder slot in inventorySlots) slot.CheckForDrop(eventData);
        foreach (InventoryPlaceHolder hotSlot in hotBar) hotSlot.CheckForDrop(eventData);
    }

    /// <summary> Set player variables </summary>
    /// <param name="player"> local player </param>
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

    /// <summary> Constructor </summary>
    /// <param name="itemIndex"> index of item prefab </param>
    /// <param name="icon"> icon for inventory panel </param>
    public InventoryItemAndIcon(int itemIndex, Sprite icon)
    {
        this.itemIndex = itemIndex;
        this.icon = icon;
    }
}
