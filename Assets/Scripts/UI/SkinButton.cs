using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinButton : MonoBehaviour
{
    public SkinInfo skinInfo;

    public void Update()
    {
        if (Input.GetKeyDown("" + skinInfo.index)) ChangeSkin();
    }

    public void ChangeSkin()
    {
        SkinSwitcher.instance.ChangeSkin(skinInfo);
    }
}
