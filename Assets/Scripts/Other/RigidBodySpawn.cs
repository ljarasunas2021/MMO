using Mirror;
using UnityEngine;

///<summary> spawn with dynamic rigidbody on server </summary>
public class RigidBodySpawn : NetworkBehaviour
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        GetComponent<Rigidbody>().isKinematic = false;
    }
}

