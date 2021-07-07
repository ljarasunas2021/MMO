using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    private Quaternion rot;
    void Start()
    {
        rot = transform.rotation;
    }

    void Update()
    {
        transform.rotation = rot;
    }
}
