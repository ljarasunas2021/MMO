using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour {
    private UIManager UIScript;
    private Camera _camera;

    void Start() {
        UIScript = GameObject.Find("UI Manager").GetComponent<UIManager>();
        _camera = GetComponent<Camera>();
    }

    // void OnGUI() {
    //     int size = 12;
	// 	float posX = _camera.pixelWidth/2 - size/4;
	// 	float posY = _camera.pixelHeight/2 - size/2;
	// 	GUI.Label(new Rect(posX, posY, size, size), "*");
    // }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 point = new Vector3(_camera.pixelWidth/2, _camera.pixelWidth/2, 0);
            Ray ray = _camera.ScreenPointToRay(point);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                GameObject hitObject = hit.transform.gameObject;
                Debug.Log("hit " + hitObject.name);
                Target target = hitObject.GetComponent<Target>();
                if (target != null) {
                    target.Interact();
                    Debug.Log("hit");
                }
            }
        }
    }
}
