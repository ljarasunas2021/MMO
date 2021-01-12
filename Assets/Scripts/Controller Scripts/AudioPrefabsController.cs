using UnityEngine;

namespace MMO
{
    ///<summary> Singleton that holds array of all audio files in game </summary>
    public class AudioPrefabsController : MonoBehaviour
    {
        // singleton instance
        public static AudioPrefabsController instance;
        // array of audio clips to play
        public AudioClip[] audioClipPrefabs;

        /// <summary> Create a singleton pattern </summary>
        private void Start()
        {
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
}
