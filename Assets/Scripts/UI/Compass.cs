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
        //Debug.Log(Vector3.forward.magnitude + " " + mainCam.transform.forward.magnitude);
        Debug.Log(Vector3.forward + " " + mainCam.transform.forward.ToString());
        //float angle = Mathf.Acos(Vector3.Dot(Vector3.forward, mainCam.transform.forward) / (Vector3.forward.magnitude * mainCam.transform.forward.magnitude)) * 180 / Mathf.PI;
        Vector3 camForward = mainCam.transform.forward;
        camForward.y = 0;
        Debug.Log(camForward);
        float angle = Vector3.Angle(Vector3.forward, camForward);
        float angle1 = angle;
        //Debug.Log(Mathf.Acos(Vector3.Dot(Vector3.forward, mainCam.transform.forward)));
        //Debug.Log(Mathf.PI);
        if (mainCam.transform.forward.x > 0) angle *= -1;
        //Debug.Log(angle1 + " to: " + angle);
        //Debug.Log(Vector3.Dot(Vector3.forward, player.transform.forward));
        rectTransform.rotation = Quaternion.Euler(angle * Vector3.forward);
    }
}
