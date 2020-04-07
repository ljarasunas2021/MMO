using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    private bool playerMode = true;
    [HideInInspector] public GameObject player;
    private Vector3 localPos, localRot, localScale;

    void Start()
    {
        localPos = transform.localPosition;
        localRot = transform.localEulerAngles;
        localScale = transform.localScale;
        player = transform.root.gameObject;
    }

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
