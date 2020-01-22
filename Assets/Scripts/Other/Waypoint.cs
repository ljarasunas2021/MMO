using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Color color;
    public Vector3 GetPosition()
    {
        return gameObject.transform.position;
    }
}
