﻿using System.Collections;
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

    public void AddWaypoint(Waypoint waypoint, MapMarker mapMarker)
    {
        wayPoints.Add(new WaypointAndInstant(waypoint));

        GameObject markerInstant = Instantiate(waypointMarker, circleParent.transform);

        markerInstant.GetComponent<Image>().color = waypoint.color;
        wayPoints[wayPoints.Count - 1].marker = markerInstant;
        wayPoints[wayPoints.Count - 1].mapMarker = mapMarker;
    }

    public void RemoveWaypoint(MapMarker mapMarker)
    {
        int index = -1;
        for (int i = 0; i < wayPoints.Count; i++)
        {
            if (wayPoints[i].mapMarker == mapMarker) index = i;
        }
        Destroy(wayPoints[index].waypoint.gameObject);
        Destroy(wayPoints[index].marker.gameObject);
        Destroy(wayPoints[index].mapMarker.gameObject);
        wayPoints.RemoveAt(index);
    }
}

[System.Serializable]
public class WaypointAndInstant
{
    public Waypoint waypoint;
    public GameObject marker;
    public MapMarker mapMarker;

    public WaypointAndInstant(Waypoint waypoint)
    {
        this.waypoint = waypoint;
    }
}
