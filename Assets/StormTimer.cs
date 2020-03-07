using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StormTimer : MonoBehaviour
{
    private Text stormText;

    private void Start()
    {
        stormText = GetComponent<Text>();
    }

    public void SetTime(int min, int sec)
    {
        string secString = (sec < 10) ? "0" + sec : "" + sec;
        stormText.text = min + ":" + secString;
    }
}

