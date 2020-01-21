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
        Vector3 camForward = mainCam.transform.forward;
        camForward.y = 0;
        float angle = Vector3.Angle(Vector3.forward, camForward);
        if (mainCam.transform.forward.x > 0) angle *= -1;
        rectTransform.rotation = Quaternion.Euler(angle * Vector3.forward);
    }
}
