using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

///<summary> Contains the different camera modes <summary>
public class CameraController : MonoBehaviour
{
    public GameObject cinematicCam, closeUpCam, lockedCamRagdoll, lockedCamNonRagdoll, busCam;

    public PostProcessProfile storm;

    [HideInInspector] public BattleRoyalePlayer battleRoyalePlayer;

    private PostProcessVolume volume;

    void Start()
    {
        volume = GetComponent<PostProcessVolume>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("storm"))
        {
            volume.profile = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("storm"))
        {
            volume.profile = storm;
        }
    }
}
