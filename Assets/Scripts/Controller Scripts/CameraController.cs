using UnityEngine;

/// <summary> Singleton which stores every camera </summary>
public class CameraController : MonoBehaviour
{
    //singleton
    public static CameraController instance;
    //different cameras
    public GameObject cinematicCam, closeUpCam, lockedCam;

    /// <summary> Create a singleton</summary>
    private void Start()
    {
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
