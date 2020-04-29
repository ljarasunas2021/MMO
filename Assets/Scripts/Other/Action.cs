using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Action1 : MonoBehaviour
{
    public IEnumerator Execute()
    {
        yield return 0;
        Debug.Log("HERE");
    }
}