using Mirror;
using UnityEngine;

///<summary> spawn with dynamic rigidbody on server </summary>
public class RigidBodySpawn : NetworkBehaviour
{
    ///<summary> Change rigidbody to dynamic on server </summary>
    public override void OnStartServer()
    {
        base.OnStartServer();
        GetComponent<Rigidbody>().isKinematic = false;
    }
}

