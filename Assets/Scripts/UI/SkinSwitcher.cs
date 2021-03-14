using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinSwitcher : MonoBehaviour
{
    public static SkinSwitcher instance;
    public GameObject parent;

    private void Start()
    {
        instance = this;
    }

    public void ChangeSkin(SkinInfo skinInfo)
    {
        foreach (ChangeSkin changeSkin in FindObjectsOfType<ChangeSkin>())
        {
            if (changeSkin.isLocalPlayer)
            {
                changeSkin.ChangeSkinFunc(skinInfo);
            }
        }
    }

    public void Toggle()
    {
        parent.SetActive(!parent.activeSelf);
    }
}

[System.Serializable]
public class SkinInfo
{
    public string name;
    public Avatar avatar;
    public int index;
}
