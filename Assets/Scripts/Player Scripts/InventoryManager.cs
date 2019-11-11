using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///<summary> Manage the player's inventory</summary>
public class InventoryManager : MonoBehaviour
{
    #region Variables
    // deafult inventory image
    public Image inventoryImage;
    // vertical slots in inventory count
    public int slotsInVertical;
    // space between each slot
    public float spaceBetweenSlots;
    // if the inventory is showing
    private bool inventoryEnabled = false;
    // width / height of the inventory image
    private float inventoryImageWidth, inventoryImageHeight;
    // image script of the inventory
    private Image imageScript;
    // the inventory gameObject
    private GameObject inventory;
    // the rect transform component of the inventory
    private RectTransform rT;
    // a list of all of the current inventory items
    private List<InventoryItemAndIcon> inventoryItems = new List<InventoryItemAndIcon>();
    private UIManager uIScript;
    #endregion

    #region Initialize
    ///<summary> Set components </summary>
    private void Start()
    {
        uIScript = GameObject.FindObjectOfType<UIManager>();
        inventory = uIScript.inventory;

        imageScript = inventory.GetComponent<Image>();
        rT = inventory.GetComponent<RectTransform>();

        inventoryImageHeight = (Screen.height - slotsInVertical * spaceBetweenSlots - spaceBetweenSlots) / slotsInVertical;
        inventoryImageWidth = inventoryImageHeight;

        inventoryImage.GetComponent<RectTransform>().sizeDelta = new Vector2(inventoryImageWidth, inventoryImageHeight);
    }
    #endregion

    #region InventoryVoids
    ///<summary> Change if the inventory is shown or not shown</summary>
    public void ChangeEnabled()
    {
        inventoryEnabled = !inventoryEnabled;

        if (inventoryEnabled) ShowInventory();
        else RemoveInventory();
    }

    ///<summary> Show the inventory</summary>
    private void ShowInventory()
    {
        imageScript.enabled = true;

        float rows = Mathf.Ceil((float)inventoryItems.Count / (float)slotsInVertical);
        float futureWidth = spaceBetweenSlots * rows + rows * inventoryImageWidth + spaceBetweenSlots;
        inventory.transform.position = new Vector3(Screen.width - futureWidth / 2, Screen.height / 2, 0);

        float currentWidth = Screen.width - (inventoryImageWidth + 2 * spaceBetweenSlots);
        float currentHeight = Screen.height - spaceBetweenSlots;

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            float xPos = currentWidth + spaceBetweenSlots + inventoryImageWidth / 2;
            float yPos = currentHeight - inventoryImageHeight / 2;
            Vector3 localPosition = new Vector3(xPos, yPos, 0);
            Image image = Instantiate(inventoryImage, localPosition, Quaternion.Euler(Vector3.zero), inventory.transform);
            image.sprite = inventoryItems[i].icon;
            if ((i + 1) % slotsInVertical != 0) { currentHeight -= spaceBetweenSlots + inventoryImageHeight; }
            else
            {
                currentWidth -= inventoryImageWidth + spaceBetweenSlots;
                currentHeight = Screen.height - spaceBetweenSlots;
            }
        }

        rT.sizeDelta = new Vector3(Screen.width - currentWidth, Screen.height, 0);
    }

    ///<summary> Hide the inventory</summary>
    private void RemoveInventory()
    {
        imageScript.enabled = false;

        for (int i = 0; i < inventory.transform.childCount; i++) { Destroy(inventory.transform.GetChild(i).gameObject); }
    }

    ///<summary> Add item to inventory</summary>
    public void AddInventoryItem(GameObject item, Sprite icon) { inventoryItems.Add(new InventoryItemAndIcon(item, icon)); }
    #endregion
}

#region InventoryItemAndIcon
[System.Serializable]
///<summary> The inventory item and its icon </summary>
public class InventoryItemAndIcon
{
    public GameObject item;
    public Sprite icon;

    public InventoryItemAndIcon(GameObject item, Sprite icon)
    {
        this.item = item;
        this.icon = icon;
    }
}
#endregion
