using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Image inventoryImage;

    public int slotsInVertical;

    public float spaceBetweenSlots;

    private bool inventoryEnabled = false;

    private float inventoryImageWidth, inventoryImageHeight;

    private Image imageScript;

    private RectTransform rT;

    private List<InventoryItemAndIcon> inventoryItems = new List<InventoryItemAndIcon>();

    private void Start()
    {
        imageScript = GetComponent<Image>();
        rT = GetComponent<RectTransform>();

        inventoryImageHeight = (Screen.height - slotsInVertical * spaceBetweenSlots - spaceBetweenSlots) / slotsInVertical;
        inventoryImageWidth = inventoryImageHeight;
        inventoryImage.GetComponent<RectTransform>().sizeDelta = new Vector2(inventoryImageWidth, inventoryImageHeight);

        ShowInventory();
    }

    public void ChangeEnabled(bool enabled)
    {
        this.inventoryEnabled = enabled;
        if (enabled) ShowInventory();
        else RemoveInventory();
    }

    private void ShowInventory()
    {
        imageScript.enabled = true;

        float rows = Mathf.Ceil((float)inventoryItems.Count / (float)slotsInVertical);
        float futureWidth = spaceBetweenSlots * rows + rows * inventoryImageWidth + spaceBetweenSlots;
        transform.position = new Vector3(Screen.width - futureWidth / 2, Screen.height / 2, 0);

        float currentWidth = Screen.width - (inventoryImageWidth + 2 * spaceBetweenSlots);
        float currentHeight = Screen.height - spaceBetweenSlots;

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            float xPos = currentWidth + spaceBetweenSlots + inventoryImageWidth / 2;
            float yPos = currentHeight - inventoryImageHeight / 2;
            Vector3 localPosition = new Vector3(xPos, yPos, 0);
            Image image = Instantiate(inventoryImage, localPosition, Quaternion.Euler(Vector3.zero), transform);
            image.sprite = inventoryItems[i].icon;
            if ((i + 1) % slotsInVertical != 0)
            {
                currentHeight -= spaceBetweenSlots + inventoryImageHeight;
            }
            else
            {
                currentWidth -= inventoryImageWidth + spaceBetweenSlots;
                currentHeight = Screen.height - spaceBetweenSlots;
            }
        }

        rT.sizeDelta = new Vector3(Screen.width - currentWidth, Screen.height, 0);
    }

    private void RemoveInventory()
    {
        imageScript.enabled = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i));
        }
    }

    public void AddInventoryItem(GameObject item, Sprite icon)
    {
        inventoryItems.Add(new InventoryItemAndIcon(item, icon));
    }
}

[System.Serializable]
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
