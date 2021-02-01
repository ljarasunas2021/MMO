using Cinemachine;
using Mirror;
using UnityEngine;

///<summary> Preform all actions related to player and the camera </summary>
public class PlayerCameraManager : NetworkBehaviour
{
    // player gameobject
    private GameObject head;
    private GameObject lockedCamFollow;

    // current camera
    private CameraModes currentCam;
    // camera controller script
    private CameraController cameraController;
    // cinemchine free look cameras
    private CinemachineFreeLook cinematicFreeLook, closeUpFreeLook, lockedFreeLook;
    // movement script of player
    private Movement movement;
    // body parts script of player
    private BodyParts bodyParts;

    /// <summary> Init vars </summary>
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

    /// <summary> Change the current camera </summary>
    /// <param name="mode"> the new current camera mode </param>
    public void ChangeCam(CameraModes mode)
    {
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

    /// <summary> Return current camera mode </summary>
    /// <returns> current camera mode </returns>
    public CameraModes ReturnCameraMode() { return currentCam; }
}

/// <summary> 3 different camera modes </summary>
public enum CameraModes
{
    cinematic,
    closeUp,
    locked,
}