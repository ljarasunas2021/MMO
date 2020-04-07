using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryPlaceHolder : MonoBehaviour, IDragHandler, IEndDragHandler
{
    // is a hot bar slot
    public bool isHotBarPlaceHolder = false;
    // start position
    [HideInInspector] public Vector3 startPos;
    // the item and icon that is in this inventory slot
    [HideInInspector] public InventoryItemAndIcon itemAndIcon;
    // image that can be changed to display the icon
    [HideInInspector] public Image image;

    // the inventory manager script
    private InventoryManager inventoryManager;
    // index if it is a hot bar
    private int hotBarIndex = -1;

    // initiate values of vars
    void Start()
    {
        startPos = transform.position;
        inventoryManager = GameObject.FindObjectOfType<InventoryManager>();
        image = GetComponent<Image>();
    }

    // make it look like its being dragged
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    // Check all cases by calling end drag, bring to start position
    public void OnEndDrag(PointerEventData eventData)
    {
        inventoryManager.EndDrag(eventData);
        transform.position = startPos;
    }

    // if item dropped over another switch values and images
    public void CheckForDrop(PointerEventData eventData)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint((RectTransform)transform, Input.mousePosition)) return;

        GameObject droppedItem = eventData.pointerDrag;
        InventoryPlaceHolder placeHolder = droppedItem.GetComponent<InventoryPlaceHolder>();

        InventoryItemAndIcon placeHolderItemAndIcon = placeHolder.itemAndIcon;
        placeHolder.SetItemAndIcon(itemAndIcon);
        SetItemAndIcon(placeHolderItemAndIcon);

        //if (isHotBarPlaceHolder) { inventoryManager.EnableEquip(hotBarIndex, itemAndIcon.itemIndex); }
    }

    // set the item an icon
    public void SetItemAndIcon(InventoryItemAndIcon itemAndIcon)
    {
        this.itemAndIcon = itemAndIcon;
        image.sprite = (itemAndIcon.icon != null) ? itemAndIcon.icon : inventoryManager.defaultSprite;
    }

    // set the hot bar index
    public void SetHotBarIndex(int hotBarIndex)
    {
        this.hotBarIndex = hotBarIndex;
        itemAndIcon = new InventoryItemAndIcon(-1, null);
    }
}
