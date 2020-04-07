using Cinemachine;
using Mirror;
using UnityEngine;

///<summary> Preform all actions related to player and the camera </summary>
public class PlayerCameraManager : NetworkBehaviour
{
    private GameObject head;
    private GameObject lockedCamFollow;
    private CameraModes currentCam;
    private CameraController cameraController;
    private CinemachineFreeLook cinematicFreeLook, closeUpFreeLook, lockedFreeLook;
    private Movement movement;
    private BodyParts bodyParts;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (!isLocalPlayer) return;

        bodyParts = GetComponent<BodyParts>();
        head = bodyParts.head;
        cameraController = Camera.main.GetComponent<CameraController>();
        movement = GetComponent<Movement>();
        lockedCamFollow = bodyParts.lockedCamFollow;

        cinematicFreeLook = cameraController.cinematicCam.GetComponent<CinemachineFreeLook>();
        cinematicFreeLook.Follow = head.transform;
        cinematicFreeLook.LookAt = head.transform;

        closeUpFreeLook = cameraController.closeUpCam.GetComponent<CinemachineFreeLook>();
        closeUpFreeLook.Follow = head.transform;
        closeUpFreeLook.LookAt = head.transform;

        lockedFreeLook = cameraController.lockedCam.GetComponent<CinemachineFreeLook>();
        lockedFreeLook.Follow = lockedCamFollow.transform;
        lockedFreeLook.LookAt = lockedCamFollow.transform;

        GameObject.FindObjectOfType<Compass>().player = gameObject;
        GameObject.FindObjectOfType<Map>().player = gameObject;

        ChangeCam(CameraModes.cinematic);
    }

    public void ChangeCam(CameraModes mode)
    {
        Debug.Log(mode);

        if (mode != currentCam)
        {
            if (mode == CameraModes.cinematic)
            {
                cinematicFreeLook.Priority = 1;
                closeUpFreeLook.Priority = 0;
                lockedFreeLook.Priority = 0;
            }
            else if (mode == CameraModes.closeUp)
            {
                cinematicFreeLook.Priority = 0;
                closeUpFreeLook.Priority = 1;
                lockedFreeLook.Priority = 0;
            }
            else if (mode == CameraModes.locked)
            {
                cinematicFreeLook.Priority = 0;
                closeUpFreeLook.Priority = 0;
                lockedFreeLook.Priority = 1;
            }

            currentCam = mode;

            movement.SetCurrentCam(currentCam);
        }
    }

    public CameraModes ReturnCameraMode() { return currentCam; }
}

public enum CameraModes
{
    cinematic,
    closeUp,
    locked,
    bus
}