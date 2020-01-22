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
    private CinemachineFreeLook lockedFreeLookRagdoll, lockedFreeLookNonRagdoll;

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
        cameraController = Camera.main.GetComponent<CameraController>();
        movement = nonRagdoll.GetComponent<Movement>();
        lockedCamFollow = (movement.physicsBasedMovement) ? bodyParts.ragdollLockedCamFollow : bodyParts.nonRagdollLockedCamFollow;

        cinematicFreeLook = cameraController.cinematicCam.GetComponent<CinemachineFreeLook>();
        cinematicFreeLook.Follow = head.transform;
        cinematicFreeLook.LookAt = head.transform;

        closeUpFreeLook = cameraController.closeUpCam.GetComponent<CinemachineFreeLook>();
        closeUpFreeLook.Follow = head.transform;
        closeUpFreeLook.LookAt = head.transform;

        lockedFreeLookRagdoll = cameraController.lockedCamRagdoll.GetComponent<CinemachineFreeLook>();
        lockedFreeLookRagdoll.Follow = lockedCamFollow.transform;
        lockedFreeLookRagdoll.LookAt = lockedCamFollow.transform;

        lockedFreeLookNonRagdoll = cameraController.lockedCamNonRagdoll.GetComponent<CinemachineFreeLook>();
        lockedFreeLookNonRagdoll.Follow = lockedCamFollow.transform;
        lockedFreeLookNonRagdoll.LookAt = lockedCamFollow.transform;

        GameObject.FindObjectOfType<Compass>().Initialize(nonRagdoll);

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
                lockedFreeLookRagdoll.Priority = 0;
                lockedFreeLookNonRagdoll.Priority = 0;
            }
            else if (mode == CameraModes.closeUp)
            {
                cinematicFreeLook.Priority = 0;
                closeUpFreeLook.Priority = 1;
                lockedFreeLookRagdoll.Priority = 0;
                lockedFreeLookNonRagdoll.Priority = 0;
            }
            else if (mode == CameraModes.locked)
            {
                cinematicFreeLook.Priority = 0;
                closeUpFreeLook.Priority = 0;

                if (movement.physicsBasedMovement)
                {
                    lockedFreeLookRagdoll.Priority = 1;
                    lockedFreeLookNonRagdoll.Priority = 0;
                }
                else
                {
                    lockedFreeLookRagdoll.Priority = 0;
                    lockedFreeLookNonRagdoll.Priority = 1;
                }
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