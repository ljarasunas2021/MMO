using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//base action for dialogue system
[System.Serializable]
public class Action1 : MonoBehaviour {
    public virtual IEnumerator Execute() {
        yield return 0;
        Debug.Log("HERE");
    }
}
