using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class CameraManager : NetworkBehaviour
{
    public override void OnStartLocalPlayer()
    {
        if (!isLocalPlayer) return;
        CinemachineFreeLook freeLook = Camera.main.transform.parent.GetChild(1).GetComponent<CinemachineFreeLook>();
        freeLook.Follow = transform;
        freeLook.LookAt = transform;
    }
}
