using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    //public GameObject player;
    private RectTransform rectTransform;
    private Camera mainCam;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCam = Camera.main;
    }

    void Update()
    {
        //if (player == null) return;

        float angle = Mathf.Acos(Vector3.Dot(Vector3.forward, mainCam.transform.forward)) * 180 / Mathf.PI;
        if (mainCam.transform.forward.x > 0) angle *= -1;
        Debug.Log(angle);
        //Debug.Log(Vector3.Dot(Vector3.forward, player.transform.forward));
        rectTransform.rotation = Quaternion.Euler(angle * Vector3.forward);
    }
}
