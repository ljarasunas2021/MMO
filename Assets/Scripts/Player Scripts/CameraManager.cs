using Cinemachine;
using Mirror;
using UnityEngine;

///<summary> Preform all actions related to player and the camera </summary>
public class CameraManager : NetworkBehaviour {
    
    private bool serverStarted = false;
  
    #region SetCameraAtStart
    ///<summary> Set the camera to follow you </summary>
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (!isLocalPlayer) {
            return;
        }

        CinemachineFreeLook freeLook = Camera.main.transform.parent.GetChild(1).GetComponent<CinemachineFreeLook>();
        freeLook.Follow = transform;
        freeLook.LookAt = transform;
        
        serverStarted = true;
    }
    #endregion

    void OnGUI() {
        if (serverStarted) {
            int size = 12;
            float posX = Camera.main.pixelWidth/2 - size/4;
            float posY = Camera.main.pixelHeight/2 - size/2;
            GUI.Label(new Rect(posX, posY, size, size), "*");
        }
    }
}