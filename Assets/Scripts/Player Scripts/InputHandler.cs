using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InputHandler : NetworkBehaviour
{
    private Movement movement;
    private GrabItem grabItem;

    // transform of the camera
    private Transform camTransform;

    void Start()
    {
        movement = GetComponent<Movement>();
        grabItem = GetComponent<GrabItem>();
        camTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        InputStruct input = new InputStruct(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), Input.GetKey(KeyCode.Space), Input.GetKey(KeyCode.LeftShift), Input.GetKey(KeyCode.LeftControl), Input.GetKeyDown(KeyCode.E), camTransform.eulerAngles.y);

        movement.Move(input);
        grabItem.Grab(input);

    }
}

#region  InputStruct
/// <summary> Struct where input is stored </summary>
public struct InputStruct
{
    public float horAxis;
    public float vertAxis;
    public bool space;
    public bool leftShift;
    public bool leftControl;
    public bool eDown;
    public float camYRot;

    public InputStruct(float horAxis, float vertAxis, bool space, bool leftShift, bool leftControl, bool eDown, float camYRot)
    {
        this.horAxis = horAxis;
        this.vertAxis = vertAxis;
        this.space = space;
        this.leftShift = leftShift;
        this.leftControl = leftControl;
        this.eDown = eDown;
        this.camYRot = camYRot;
    }
}
#endregion
