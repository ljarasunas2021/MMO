using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SchoolBus : NetworkBehaviour
{
    public float radius, speed, centerRadius;
    [HideInInspector] public bool activatedBus = false;
    private PlayersController playersController;
    public Transform dropPos;

    private void Start()
    {
        playersController = GameObject.FindObjectOfType<PlayersController>();
    }

    public void ActivateBus()
    {
        float theta = Random.Range(0, 2 * Mathf.PI);
        float theta2 = Random.Range(0, 2 * Mathf.PI);
        transform.position = new Vector3(Mathf.Cos(theta) * radius, transform.position.y, Mathf.Sin(theta) * radius);
        float distance = Random.Range(0, centerRadius);
        Vector3 center = new Vector3(Mathf.Cos(theta2) * distance, transform.position.y, Mathf.Sin(theta2) * distance);
        Quaternion rot = Quaternion.LookRotation(center - transform.position, Vector3.up);
        transform.rotation = Quaternion.Euler(rot.eulerAngles /*+ Vector3.up * 90*/);
        activatedBus = true;
        foreach (GameObject player in playersController.players)
        {
            player.GetComponent<PlayerCameraManager>().ChangeCam(CameraModes.bus);
        }
    }

    void Update()
    {
        if (activatedBus)
        {
            transform.Translate(Vector3.forward * speed);
        }
    }
}
