using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Singleton which stores every player </summary>
public class PlayersController : MonoBehaviour
{
    // singleton
    public static PlayersController instance;

    // list of all players
    public List<GameObject> players;

    private void Start()
    {
        //singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("There is already an instance of the Players Controller.");
        }
    }
}
