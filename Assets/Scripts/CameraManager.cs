using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEngine;

public class CameraManager : NetworkBehaviour {
    public override void OnStartLocalPlayer () {
        base.OnStartLocalPlayer ();
        if (!isLocalPlayer) return;
        CinemachineFreeLook freeLook = Camera.main.transform.parent.GetChild (1).GetComponent<CinemachineFreeLook> ();
        freeLook.Follow = transform;
        freeLook.LookAt = transform;
    }
}