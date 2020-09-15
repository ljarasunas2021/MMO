using UnityEngine;

///<summary> Array of effects for commands to use </summary>
public class EffectsPrefabsController : MonoBehaviour
{
    //singleton
    public static EffectsPrefabsController instance;

    // array of effects
    public GameObject[] effectPrefabs;

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
