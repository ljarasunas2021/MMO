using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

/// <summary> Manage the compass UI </summary>
public class Compass : NetworkBehaviour
{
    // player GameObject
    [HideInInspector] public GameObject player;
    // ui waypoint marker
    public GameObject waypointMarker;
    // the gameObject that rotates the minimap and the waypoints
    public GameObject circleParent;
    // the mask that makes the minimap circular
    public GameObject mask;
    // a list of the player's waypoints
    public List<WaypointAndInstant> wayPoints = new List<WaypointAndInstant>();
    // the transform of Camera.main
    private Transform mainCamTransform;
    // the width of the compass
    private float width;
    // singleton instance var
    public static Compass instance;

    /// <summary> Init vars </summary>
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There already exists an instance of the compass script");
        }

        mainCamTransform = Camera.main.transform;
        width = mask.GetComponent<RectTransform>().rect.width / 2;
    }

    /// <summary> Rotate the compass accordingly </summary>
    void Update()
    {
        if (player == null) return;

        Vector3 forward = player.transform.forward;
        forward.y = 0;
        float rot1 = Vector3.SignedAngle(Vector3.forward, forward, Vector3.up);

        foreach (WaypointAndInstant waypoint in wayPoints)
        {
            Vector3 diff = waypoint.waypoint.transform.position - player.transform.position;
            diff.y = 0;
            float rot = Vector3.SignedAngle(player.transform.forward, diff, Vector3.up);
            rot *= Mathf.PI / 180;
            Vector2 localPosition = new Vector2(Mathf.Sin(rot) * width, Mathf.Cos(rot) * width);
            waypoint.compassMarker.transform.localPosition = localPosition;
        }
    }

    /// <summary> Add a waypoint to the waypoint list, display waypoint appropriately </summary>
    /// <param name="waypoint"> the waypoint (empty gameobject in space that markers should point to) </param>
    /// <param name="mapMarker"> the waypoints marker on the map </param>
    public void AddWaypoint(Waypoint waypoint, MapMarker mapMarker)
    {
        wayPoints.Add(new WaypointAndInstant(waypoint, null, mapMarker));

        GameObject markerInstant = Instantiate(waypointMarker, circleParent.transform);

        markerInstant.GetComponent<Image>().color = waypoint.color;
        wayPoints[wayPoints.Count - 1].compassMarker = markerInstant;
    }

    /// <summary> Remove a waypoint and destroy the appropriate gameobjects </summary>
    /// <param name="mapMarker"> map marker of waypoint to delete </param>
    public void RemoveWaypoint(MapMarker mapMarker)
    {
        int index = -1;
        for (int i = 0; i < wayPoints.Count; i++)
        {
            if (wayPoints[i].mapMarker == mapMarker) index = i;
        }
        Destroy(wayPoints[index].waypoint.gameObject);
        Destroy(wayPoints[index].compassMarker.gameObject);
        Destroy(wayPoints[index].mapMarker.gameObject);
        wayPoints.RemoveAt(index);
    }
}

/// <summary> Class that holds the waypoint and its instant </summary>
[System.Serializable]
public class WaypointAndInstant
{
    // the waypoint itself
    public Waypoint waypoint;
    // marker on the map
    public GameObject compassMarker;
    // marker on the compass
    public MapMarker mapMarker;

    /// <summary> Constructor </summary>
    /// <param name="waypoint"> the waypoint (empty gameobject in space that markers should point to) </param>
    /// <param name="compassMarker"> marker on the map</param>
    /// <param name="mapMarker"> marker on the compass </param>
    public WaypointAndInstant(Waypoint waypoint, GameObject compassMarker, MapMarker mapMarker)
    {
        this.waypoint = waypoint;
        this.mapMarker = mapMarker;
        this.compassMarker = compassMarker;
    }
}
