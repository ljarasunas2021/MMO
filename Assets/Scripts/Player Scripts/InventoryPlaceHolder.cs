using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryPlaceHolder : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public bool isHotBarPlaceHolder = false;
    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public InventoryItemAndIcon itemAndIcon;
    [HideInInspector] public Image image;

    private InventoryManager2 inventoryManager;
    private int hotBarIndex;

    void Start()
    {
        startPos = transform.position;
        inventoryManager = GameObject.FindObjectOfType<InventoryManager2>();
        image = GetComponent<Image>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        inventoryManager.EndDrag(eventData);
        transform.position = startPos;
    }

    public void CheckForDrop(PointerEventData eventData)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint((RectTransform)transform, Input.mousePosition)) return;

        GameObject droppedItem = eventData.pointerDrag;
        InventoryPlaceHolder placeHolder = droppedItem.GetComponent<InventoryPlaceHolder>();

        InventoryItemAndIcon placeHolderItemAndIcon = placeHolder.itemAndIcon;
        placeHolder.SetItemAndIcon(itemAndIcon);
        SetItemAndIcon(placeHolderItemAndIcon);

        if (isHotBarPlaceHolder) { inventoryManager.EquipItem(hotBarIndex, itemAndIcon.itemIndex); }
    }

    public void SetItemAndIcon(InventoryItemAndIcon itemAndIcon)
    {
        this.itemAndIcon = itemAndIcon;
        image.sprite = (itemAndIcon.icon != null) ? itemAndIcon.icon : inventoryManager.defaultSprite;
    }

    public void SetHotBarIndex(int hotBarIndex)
    {
        this.hotBarIndex = hotBarIndex;
        itemAndIcon = new InventoryItemAndIcon(-1, null);
    }
}
