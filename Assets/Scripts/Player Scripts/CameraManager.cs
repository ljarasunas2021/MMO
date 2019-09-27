using Cinemachine;
using Mirror;
using UnityEngine;

///<summary> Preform all actions related to player and the camera </summary>
public class CameraManager : NetworkBehaviour
{
    #region SetCameraAtStart
    ///<summary> Set the camera to follow you </summary>
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (!isLocalPlayer) return;

        CinemachineFreeLook freeLook = Camera.main.transform.parent.GetChild(1).GetComponent<CinemachineFreeLook>();
        freeLook.Follow = transform;
        freeLook.LookAt = transform;
    }
    #endregion
}