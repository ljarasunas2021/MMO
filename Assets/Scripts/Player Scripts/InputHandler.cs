﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InputHandler : NetworkBehaviour
{
    private ItemHolding itemHolding;

    private Movement movement;
    private PlayerEquip playerEquip;
    private InventoryManager inventoryManager;
    private Transform camTransform;

    void Start()
    {
        movement = GetComponent<Movement>();
        playerEquip = GetComponent<PlayerEquip>();
        inventoryManager = GetComponent<InventoryManager>();
        camTransform = Camera.main.transform;
        itemHolding = new ItemHolding(null, HoldingItemType.none);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        InputStruct input = new InputStruct(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), Input.GetButton("Jump"), Input.GetButton("Sprint"), Input.GetButton("Free Rotate Camera"), Input.GetButtonDown("Pickup"), Input.GetButtonDown("Inventory"), Input.GetButton("Fire1"), Input.GetButtonUp("Fire1"), Input.GetButtonDown("Reload"), Input.GetButton("Cancel"), Input.mousePosition, camTransform.eulerAngles.y);

        movement.Move(input);
        TestGrab(input);
        TestUI(input);
        CheckItemHolding(input);
    }

    private void TestGrab(InputStruct input)
    {
        if (input.pickupDown) playerEquip.Grab();
    }

    private void TestUI(InputStruct input)
    {
        if (input.switchInventoryDown) inventoryManager.ChangeEnabled();
    }

    private void CheckItemHolding(InputStruct input)
    {
        if (itemHolding.type == HoldingItemType.ranged)
        {
            itemHolding.weaponScript.CheckForUserInput(input);
        }

        if (itemHolding.type == HoldingItemType.melee)
        {

        }

        if (itemHolding.type == HoldingItemType.collectable)
        {

        }
    }

    public void ChangeItemHolding(ItemHolding itemHolding)
    {
        this.itemHolding = itemHolding;
    }
}

#region  InputStruct
/// <summary> Struct where input is stored </summary>
public struct InputStruct
{
    public float horAxis, vertAxis, camYRot;
    public bool jump, sprint, freeRotateCamera, pickupDown, switchInventoryDown, fire1, fire1Up, reloadDown, cancel;
    public Vector2 mousePos;

    public InputStruct(float horAxis, float vertAxis, bool jump, bool sprint, bool freeRotateCamera, bool pickUpDown, bool switchInventoryDown, bool fire1, bool fire1Up, bool reloadDown, bool cancel, Vector2 mousePos, float camYRot)
    {
        this.horAxis = horAxis;
        this.vertAxis = vertAxis;
        this.jump = jump;
        this.sprint = sprint;
        this.freeRotateCamera = freeRotateCamera;
        this.pickupDown = pickUpDown;
        this.switchInventoryDown = switchInventoryDown;
        this.fire1 = fire1;
        this.fire1Up = fire1Up;
        this.reloadDown = reloadDown;
        this.cancel = cancel;
        this.camYRot = camYRot;
        this.mousePos = mousePos;
    }
}
#endregion
