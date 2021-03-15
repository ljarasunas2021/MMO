using Mirror;
using UnityEngine;

///<summary> spawn with dynamic rigidbody on server </summary>
public class RigidBodySpawn : NetworkBehaviour
{
    ///<summary> Change rigidbody to dynamic on server (keeping it kinematic on all clients) </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();
        GetComponent<Rigidbody>().isKinematic = false;
    }
}

