using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ChangeSkin : NetworkBehaviour
{
   public void ChangeSkinFunc(SkinInfo skinInfo)
    {

        Vector3 pos = Vector3.zero;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                pos = child.localPosition;
            }
        }

        foreach (Transform child in transform)
        {
            if (child.name == skinInfo.name)
            {
                child.gameObject.SetActive(true);
                GetComponent<Animator>().avatar = skinInfo.avatar;
                child.localPosition = pos;
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
