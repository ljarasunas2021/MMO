using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    // public float speed = 1000.0f;
    // public GameObject orbit;
    // public GameObject camera;
    // private float time;
    public GameObject target;
    public float speedMod = 10.0f;
    private Vector3 point;
    // Start is called before the first frame update
    void Start() {
        // time = 0f;
        point = target.transform.position;
        transform.LookAt(point);
        transform.Rotate(0, 180f, 0);
    }
    void Update()
    {
        // transform.position = orbit.transform.position + new Vector3(Mathf.Cos(time), 0, Mathf.Sin(time));
        // Vector3 forward = camera.transform.position - orbit.transform.position;
        // Quaternion rotation = Quaternion.LookRotation(forward, new Vector3(0f, 0.5f, 0f));
        // transform.rotation = rotation;
        // transform.eulerAngles = Quaternion.Euler(30f, 0f, 0f).eulerAngles;
        // time += Time.deltaTime / speed;
        transform.RotateAround(point, new Vector3(0.0f, 1.0f, 0.0f), 20*Time.deltaTime * speedMod);
    }
}