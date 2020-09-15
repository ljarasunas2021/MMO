using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// minimap camera
public class MinimapCamera : MonoBehaviour
{
    // player gameobject
    [HideInInspector] public GameObject player;
    // local ____ at start
    private Vector3 localPos, localRot, localScale;

    // init vars
    void Start()
    {
        localPos = transform.localPosition;
        localRot = transform.localEulerAngles;
        localScale = transform.localScale;
        player = transform.root.gameObject;
    }

    // change parent of minimap
    public void ChangeParent(Transform parent)
    {
        transform.parent = parent;
        transform.localPosition = localPos;
        transform.localRotation = Quaternion.Euler(localRot);
        transform.localScale = localScale;

        foreach (Renderer rend in player.GetComponentsInChildren<Renderer>())
        {
            rend.enabled = (parent.gameObject.layer == LayerMaskController.playerNonRagdoll);
        }
    }
}
