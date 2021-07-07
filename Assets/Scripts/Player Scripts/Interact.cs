using UnityEngine;
using Mirror;

public class Interact : NetworkBehaviour
{
    private UIManager UIScript;
    private Camera cam;

    void Start()
    {
        UIScript = GameObject.FindObjectOfType<UIManager>();
        cam = Camera.main;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 point = new Vector3(cam.pixelWidth / 2, cam.pixelHeight / 2, 0);
            Ray ray = cam.ScreenPointToRay(point);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Target target = hit.transform.gameObject.GetComponent<Target>();
                if (target != null) target.Interact();
            }
        }
    }
}
