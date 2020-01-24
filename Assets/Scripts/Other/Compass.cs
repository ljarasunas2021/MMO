using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    public GameObject waypointMarker, circleParent, mask;
    public List<WaypointAndInstant> wayPoints = new List<WaypointAndInstant>();
    private Transform mainCamTransform;
    private float width;

    public void Initialize(GameObject player)
    {
        this.player = player;
        for (int i = 0; i < wayPoints.Count; i++) AddWaypoint(i);
    }

    void Start()
    {
        mainCamTransform = Camera.main.transform;
        width = mask.GetComponent<RectTransform>().rect.width / 2;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 forward = mainCamTransform.forward;
        forward.y = 0;
        float rot1 = Vector3.Angle(Vector3.forward, forward);
        if (forward.x < 0) rot1 *= -1;
        circleParent.transform.rotation = Quaternion.Euler(Vector3.forward * rot1);

        foreach (WaypointAndInstant waypoint in wayPoints)
        {
            Vector3 diff = waypoint.waypoint.GetPosition() - player.transform.position;
            diff.y = 0;
            float rot = Vector3.Angle(diff, Vector3.forward);
            if (Vector3.forward.x - diff.x > 0) rot *= -1;
            rot *= Mathf.PI / 180;
            Vector2 localPosition = new Vector2(Mathf.Sin(rot) * width, Mathf.Cos(rot) * width);
            waypoint.marker.transform.localPosition = localPosition;
        }
    }

    public void AddWaypoint(int waypointIndex)
    {
        GameObject waypt = Instantiate(waypointMarker, circleParent.transform);

        waypt.GetComponent<Image>().color = wayPoints[waypointIndex].waypoint.GetComponent<Waypoint>().color;
        wayPoints[waypointIndex].marker = waypt;
    }
}

[System.Serializable]
public class WaypointAndInstant
{
    public Waypoint waypoint;
    public GameObject marker;
}
