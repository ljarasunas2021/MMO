using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    public List<Hair> crossHairs;

    public float size, maxDistance, crossHairTime, maxCrossHairVelo, sizeTime, maxSizeVelo, maxSize;

    private float currentDistance = 0, crossHairVelo, currentSize, sizeVelo;

    private void Update()
    {
        float targetDistance;
        RaycastHit hit;

        Ray centerRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        targetDistance = (Physics.Raycast(centerRay, out hit, maxDistance)) ? hit.distance : maxDistance;

        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref crossHairVelo, crossHairTime, maxCrossHairVelo);

        currentSize = Mathf.Clamp(Mathf.SmoothDamp(currentSize, Mathf.Abs(targetDistance - currentDistance), ref sizeVelo, sizeTime, maxSizeVelo), 0, maxSize);

        foreach (Hair crossHair in crossHairs)
        {
            crossHair.hair.transform.localPosition = crossHair.optimalPosition + crossHair.directionToExpand * currentSize;
        }
    }
}

[System.Serializable]
public class Hair
{
    public GameObject hair;
    public Vector3 optimalPosition;
    public Vector3 directionToExpand;
}
