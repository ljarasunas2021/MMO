using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    private UIManager UIScript;
    private Camera _camera;

    void Start()
    {
        UIScript = GameObject.FindObjectOfType<UIManager>();
        _camera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);
            Ray ray = _camera.ScreenPointToRay(point);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Target target = hit.transform.gameObject.GetComponent<Target>();
                if (target != null) target.Interact();
            }
        }
    }
}
