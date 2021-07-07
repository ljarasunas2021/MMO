using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MMO.UI.Inventory
{
    /// <summary> Draggable item in inventory</summary>
    public class InventoryPlaceHolder : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        // is this a hot bar slot
        public bool isHotBarPlaceHolder = false;
        // start position
        [HideInInspector] public Vector3 startPos;
        // the item and icon that is in this inventory slot
        [HideInInspector] public InventoryItemAndIcon itemAndIcon;
        // image for displaying the icon
        [HideInInspector] public Image image;

        // the inventory manager singleton
        private InventoryManager inventoryManager;
        // index if it is a hot bar
        private int hotBarIndex = -1;

        /// <summary> Init vars </summary>
        void Start()
        {
            startPos = transform.position;
            inventoryManager = GameObject.FindObjectOfType<InventoryManager>();
            image = GetComponent<Image>();
        }

        /// <summary> Start dragging this placeholder </summary>
        /// <param name="eventData"> information about the drag </param>
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }

        /// <summary> Finish dragging this placeholder </summary>
        /// <param name="eventData"> information about the drag </param>
        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryManager.EndDrag(eventData);
            transform.position = startPos;
        }

        /// <summary> Called if another item is dropped on this one. Switches itemAndIcon's </summary>
        /// <param name="eventData"></param>
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

        /// <summary> Set the item and icon </summary>
        /// <param name="itemAndIcon"> new value of item and icon </param>
        public void SetItemAndIcon(InventoryItemAndIcon itemAndIcon)
        {
            this.itemAndIcon = itemAndIcon;
            image.sprite = (itemAndIcon.icon != null) ? itemAndIcon.icon : inventoryManager.defaultSprite;
        }

        /// <summary> Set the hot bar index </summary>
        /// <param name="hotBarIndex"> new value for hot bar index </param>
        public void SetHotBarIndex(int hotBarIndex)
        {
            this.hotBarIndex = hotBarIndex;
            itemAndIcon = new InventoryItemAndIcon(-1, null);
        }
    }
}
