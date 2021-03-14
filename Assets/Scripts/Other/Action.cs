using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Base action for the dialogue system, which further actions will override </summary>
[System.Serializable]
public class Action1 : MonoBehaviour {

    /// <summary> Execute the action </summary>
    /// <returns> an ienumerator since it is a coroutine, only use the ienumerator if you need information about the progress of a coroutine </returns>
    public virtual IEnumerator Execute() {
        yield return 0;
        Debug.LogWarning("Overide this execute function");
    }
}
