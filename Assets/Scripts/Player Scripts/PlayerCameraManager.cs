using Cinemachine;
using Mirror;
using UnityEngine;

///<summary> Preform all actions related to player and the camera </summary>
public class PlayerCameraManager : NetworkBehaviour
{
    #region Variables
    public GameObject nonRagdoll;
    // ____ body part
    // head
    private GameObject head;
    // locked camera empty gameObject
    private GameObject lockedCamFollow;

    // current camera
    private CameraModes currentCam;
    // camera controller script

    private CameraController cameraController;
    // ___ Free Look camera
    // cinematic
    private CinemachineFreeLook cinematicFreeLook;
    // closeup
    private CinemachineFreeLook closeUpFreeLook;
    // locked
    private CinemachineFreeLook lockedFreeLook;

    // movement script
    private Movement movement;
    // body parts script
    private BodyParts bodyParts;
    #endregion

    #region SetCameraAtStart
    ///<summary> Set the camera to follow you </summary>
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (!isLocalPlayer) return;

        bodyParts = GetComponent<BodyParts>();
        head = bodyParts.head;
        lockedCamFollow = bodyParts.lockedCamFollow;
        cameraController = Camera.main.GetComponent<CameraController>();
        movement = nonRagdoll.GetComponent<Movement>();

        cinematicFreeLook = cameraController.cinematicCam.GetComponent<CinemachineFreeLook>();
        cinematicFreeLook.Follow = head.transform;
        cinematicFreeLook.LookAt = head.transform;

        closeUpFreeLook = cameraController.closeUpCam.GetComponent<CinemachineFreeLook>();
        closeUpFreeLook.Follow = head.transform;
        closeUpFreeLook.LookAt = head.transform;

        lockedFreeLook = cameraController.lockedCam.GetComponent<CinemachineFreeLook>();
        lockedFreeLook.Follow = lockedCamFollow.transform;
        lockedFreeLook.LookAt = lockedCamFollow.transform;

        ChangeCam(CameraModes.cinematic);
    }
    #endregion

    #region ChangeCurrentCamera
    ///<summary> Change current camera </summary>
    ///<param name = "mode"> mode to switch to </param>
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
    #endregion

    #region ReturnCameraMode
    ///<summary> return current camera </summary>
    public CameraModes ReturnCameraMode() { return currentCam; }
    #endregion
}

#region CameraModes
///<summary> DIfferent camera modes</summary>
public enum CameraModes
{
    cinematic,
    closeUp,
    locked
}
#endregion