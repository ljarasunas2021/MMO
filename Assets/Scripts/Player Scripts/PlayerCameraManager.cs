using Cinemachine;
using Mirror;
using UnityEngine;

///<summary> Preform all actions related to player and the camera </summary>
public class PlayerCameraManager : NetworkBehaviour
{
    private GameObject head;
    private CameraModes currentCam;

    private CameraController cameraController;
    private CinemachineFreeLook cinematicFreeLook, closeUpFreeLook, lockedFreeLook;
    private CinemachineVirtualCamera lockedVirtual;

    #region SetCameraAtStart
    ///<summary> Set the camera to follow you </summary>
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (!isLocalPlayer) return;

        head = GetComponent<BodyParts>().head;
        cameraController = Camera.main.GetComponent<CameraController>();

        cinematicFreeLook = cameraController.cinematicCam.GetComponent<CinemachineFreeLook>();
        cinematicFreeLook.Follow = head.transform;
        cinematicFreeLook.LookAt = head.transform;

        closeUpFreeLook = cameraController.closeUpCam.GetComponent<CinemachineFreeLook>();
        closeUpFreeLook.Follow = head.transform;
        closeUpFreeLook.LookAt = head.transform;

        lockedVirtual = cameraController.lockedCam.GetComponent<CinemachineVirtualCamera>();
        lockedVirtual.Follow = head.transform;
        lockedVirtual.LookAt = head.transform;

        currentCam = CameraModes.cinematic;
    }
    #endregion

    public void ChangeCam(CameraModes mode)
    {
        if (mode != currentCam)
        {
            if (mode == CameraModes.cinematic)
            {
                cinematicFreeLook.Priority = 1;
                closeUpFreeLook.Priority = 0;
                //lockedFreeLook.Priority = 0;
                lockedVirtual.Priority = 0;
            }
            else if (mode == CameraModes.closeUp)
            {
                cinematicFreeLook.Priority = 0;
                closeUpFreeLook.Priority = 1;
                //lockedFreeLook.Priority = 0;
                lockedVirtual.Priority = 0;
            }
            else if (mode == CameraModes.locked)
            {
                cinematicFreeLook.Priority = 0;
                closeUpFreeLook.Priority = 0;
                //lockedFreeLook.Priority = 0;
                lockedVirtual.Priority = 1;
            }

            currentCam = mode;
        }
    }

    public CameraModes ReturnCameraMode()
    {
        return currentCam;
    }
}

public enum CameraModes
{
    cinematic,
    closeUp,
    locked
}