using UnityEngine;

/// <summary> Singleton which stores every effect prefab </summary>
public class EffectsPrefabsController : MonoBehaviour
{
    //singleton
    public static EffectsPrefabsController instance;

    // array of effects
    public GameObject[] effectPrefabs;

    /// <summary> Create a singleton</summary>
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
