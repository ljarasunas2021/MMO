using Cinemachine;
using Mirror;
using UnityEngine;

///<summary> Preform all actions related to player and the camera </summary>
public class PlayerCameraManager : NetworkBehaviour
{
    #region SetCameraAtStart
    ///<summary> Set the camera to follow you </summary>
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (!isLocalPlayer) return;

        GameObject head = GetComponent<BodyParts>().head;

        CameraController cameraController = Camera.main.GetComponent<CameraController>();

        CinemachineFreeLook cinematicFreeLook = cameraController.cinematicCam.GetComponent<CinemachineFreeLook>();
        cinematicFreeLook.Follow = head.transform;
        cinematicFreeLook.LookAt = head.transform;

        CinemachineFreeLook closeUpFreeLook = cameraController.closeUpCam.GetComponent<CinemachineFreeLook>();
        closeUpFreeLook.Follow = head.transform;
        closeUpFreeLook.LookAt = head.transform;

        CinemachineFreeLook lockedFreeLook = cameraController.lockedCam.GetComponent<CinemachineFreeLook>();
        lockedFreeLook.Follow = head.transform;
        lockedFreeLook.LookAt = head.transform;
    }
    #endregion
}