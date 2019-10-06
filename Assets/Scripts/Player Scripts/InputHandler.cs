using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InputHandler : NetworkBehaviour
{
    private Movement movement;
    private GrabItem grabItem;
    private InventoryManager inventoryManager;

    // transform of the camera
    private Transform camTransform;

    void Start()
    {
        movement = GetComponent<Movement>();
        grabItem = GetComponent<GrabItem>();
        inventoryManager = GetComponent<InventoryManager>();
        camTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        InputStruct input = new InputStruct(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), Input.GetButton("Jump"), Input.GetButton("Sprint"), Input.GetButton("Free Rotate Camera"), Input.GetButtonDown("Pickup"), Input.GetButtonDown("Switch Inventory"), camTransform.eulerAngles.y);

        movement.Move(input);
        TestGrab(input);
        TestUI(input);
    }

    private void TestGrab(InputStruct input)
    {
        if (input.pickup) grabItem.Grab();
    }

    private void TestUI(InputStruct input)
    {
        if (input.switchInventory) inventoryManager.ChangeEnabled();
    }
}

#region  InputStruct
/// <summary> Struct where input is stored </summary>
public struct InputStruct
{
    public float horAxis;
    public float vertAxis;
    public bool jump;
    public bool sprint;
    public bool freeRotateCamera;
    public bool pickup;
    public bool switchInventory;
    public float camYRot;

    public InputStruct(float horAxis, float vertAxis, bool jump, bool sprint, bool freeRotateCamera, bool pickUp, bool switchInventory, float camYRot)
    {
        this.horAxis = horAxis;
        this.vertAxis = vertAxis;
        this.jump = jump;
        this.sprint = sprint;
        this.freeRotateCamera = freeRotateCamera;
        this.pickup = pickUp;
        this.switchInventory = switchInventory;
        this.camYRot = camYRot;
    }
}
#endregion
