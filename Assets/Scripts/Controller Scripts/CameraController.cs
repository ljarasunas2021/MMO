using UnityEngine;

// this script stores the different cameras
public class CameraController : MonoBehaviour
{
    //singleton
    public static CameraController instance;
    //different cameras
    public GameObject cinematicCam, closeUpCam, lockedCam;

    private void Start()
    {
        //singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("There is already an instance of the Camera Controller.");
        }
    }
}
