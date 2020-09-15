using UnityEngine;

///<summary> Array of prefabs for commands to pull from </summary>
public class ItemPrefabsController : MonoBehaviour
{
    //singleton
    public static ItemPrefabsController instance;

    // array of the prefabs of the items
    public GameObject[] itemPrefabs;

    private void Start()
    {
        //singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("There is already an instance of the Item Prefabs Controller.");
        }
    }
}
