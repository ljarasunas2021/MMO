using UnityEngine;
using Mirror;

///<summary> Contains references to important body parts </summary>
public class BodyParts : NetworkBehaviour
{
    //Gameobject reference to the 
    // handR
    public GameObject handR;
    // head
    public GameObject head;
    // locked Camera Empty Gameobject (used for positioning the locked cam)
    public GameObject lockedCamFollow;

    public bool IsLocalPlayer() { return isLocalPlayer; }
}
