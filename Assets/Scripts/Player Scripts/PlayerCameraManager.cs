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
    private CinemachineFreeLook cinematicFreeLook, closeUpFreeLook, lockedFreeLookRagdoll, lockedFreeLookNonRagdoll, busFreeLook;
    private SchoolBus bus;
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
        bus = GameObject.FindObjectOfType<SchoolBus>();
        lockedCamFollow = bodyParts.lockedCamFollow;

        cinematicFreeLook = cameraController.cinematicCam.GetComponent<CinemachineFreeLook>();
        cinematicFreeLook.Follow = head.transform;
        cinematicFreeLook.LookAt = head.transform;

        closeUpFreeLook = cameraController.closeUpCam.GetComponent<CinemachineFreeLook>();
        closeUpFreeLook.Follow = head.transform;
        closeUpFreeLook.LookAt = head.transform;

        lockedFreeLookNonRagdoll = cameraController.lockedCam.GetComponent<CinemachineFreeLook>();
        lockedFreeLookNonRagdoll.Follow = lockedCamFollow.transform;
        lockedFreeLookNonRagdoll.LookAt = lockedCamFollow.transform;

        if (bus != null)
        {
            busFreeLook.Follow = bus.transform;
            busFreeLook.LookAt = bus.transform;
        }

        GameObject.FindObjectOfType<Compass>().player = gameObject;
        GameObject.FindObjectOfType<Map>().player = gameObject;

        ChangeCam(CameraModes.cinematic);
    }

    public void ChangeCam(CameraModes mode)
    {
        if (mode != currentCam)
        {
            if (mode == CameraModes.cinematic)
            {
                cinematicFreeLook.Priority = 1;
                closeUpFreeLook.Priority = 0;
                lockedFreeLookRagdoll.Priority = 0;
                lockedFreeLookNonRagdoll.Priority = 0;
                busFreeLook.Priority = 0;
            }
            else if (mode == CameraModes.closeUp)
            {
                cinematicFreeLook.Priority = 0;
                closeUpFreeLook.Priority = 1;
                lockedFreeLookRagdoll.Priority = 0;
                lockedFreeLookNonRagdoll.Priority = 0;
                busFreeLook.Priority = 0;
            }
            else if (mode == CameraModes.locked)
            {
                cinematicFreeLook.Priority = 0;
                closeUpFreeLook.Priority = 0;
                busFreeLook.Priority = 0;

                lockedFreeLookRagdoll.Priority = 0;
                lockedFreeLookNonRagdoll.Priority = 1;

            }
            else if (mode == CameraModes.bus)
            {
                cinematicFreeLook.Priority = 0;
                closeUpFreeLook.Priority = 0;
                lockedFreeLookRagdoll.Priority = 0;
                lockedFreeLookNonRagdoll.Priority = 0;
                busFreeLook.Priority = 1;
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