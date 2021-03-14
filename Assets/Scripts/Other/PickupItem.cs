using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public pickupItemType type;
    public float rotationSpeed;

    private void Update()
    {
        transform.eulerAngles += rotationSpeed * Time.deltaTime * Vector3.up;
    }
}

public enum pickupItemType
{
    Coin
}
