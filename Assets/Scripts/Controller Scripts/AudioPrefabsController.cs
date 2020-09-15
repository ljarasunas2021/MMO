using UnityEngine;

///<summary> Array of audio files to use as an array for commands to pull from</summary>
public class AudioPrefabsController : MonoBehaviour
{
    // singleton instance
    public static AudioPrefabsController instance;
    // array of audio clips to play
    public AudioClip[] audioClipPrefabs;

    private void Start()
    {
        //singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("There is already an instance of the Audio Prefabs Controller.");
        }
    }
}
