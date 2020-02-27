using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBus : MonoBehaviour
{
    public void ActivateBusVoid()
    {
        GameObject.FindObjectOfType<SchoolBus>().ActivateBus();
    }
}
