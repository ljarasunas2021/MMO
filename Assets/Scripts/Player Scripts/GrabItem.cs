using UnityEngine;

public class GrabItem : MonoBehaviour
{
    public int maxGrabDistance;

    private BodyParts bodyParts;
    private GameObject handR;

    private void Start()
    {
        bodyParts = GetComponent<BodyParts>();
        handR = bodyParts.handR;
    }

    public void Grab(InputStruct input)
    {
        if (input.eDown)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            if (Physics.Raycast(ray, out hit, maxGrabDistance, LayerMaskController.weapon) && hit.collider.gameObject.GetComponent<Weapon>() != null)
            {
                GameObject weapon = hit.collider.gameObject;
                weapon.GetComponent<Rigidbody>().isKinematic = true;
                weapon.transform.SetParent(handR.transform);
                weapon.transform.localPosition = Vector3.zero;
            }
        }
    }
}
