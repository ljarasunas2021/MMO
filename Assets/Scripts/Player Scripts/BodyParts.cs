using UnityEngine;
using Mirror;

///<summary> Contains references to important body parts </summary>
public class BodyParts : NetworkBehaviour
{
    //Gameobject reference to the 
    // handR
    public GameObject ragdollHandR;
    public GameObject nonragdollHandR;
    // head
    public GameObject head;
    // locked Camera Empty Gameobject (used for positioning the locked cam) on ragdoll and non ragdoll based on if physics or nonphysics movement
    public GameObject ragdollLockedCamFollow, nonRagdollLockedCamFollow;

    public bool IsLocalPlayer() { return isLocalPlayer; }
}
