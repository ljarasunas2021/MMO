using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SchoolBus : NetworkBehaviour
{
    public float radius, speed;

    public override void OnStartServer()
    {
        base.OnStartServer();
        float theta = Random.Range(0, 2 * Mathf.PI);
        transform.parent.position = new Vector3(Mathf.Cos(theta) * radius, transform.position.y, Mathf.Sin(theta) * radius);
        Quaternion rot = Quaternion.LookRotation(new Vector3(0, transform.position.y, 0) - transform.parent.position, Vector3.up);
        transform.parent.rotation = Quaternion.Euler(rot.eulerAngles + Vector3.up * 90);
    }

    void Update()
    {
        transform.parent.Translate(transform.up * speed);
    }
}
