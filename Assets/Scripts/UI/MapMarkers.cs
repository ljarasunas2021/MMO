using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Holds all of the map markers </summary>
public class MapMarkers : MonoBehaviour, IPointerClickHandler
{
    // the compass gameObject
    public Compass compass;
    // the map GameObject
    public Map map;
    // the marker prefab
    public GameObject marker;
    // the waypoint prefab
    public GameObject waypoint;
    // the main Camera
    private Camera mainCamera;
    // the height of the marker
    private float markerHeight;
    //the max coordinates of the player on the mapHeight
    private float maxX, minX, maxZ, minZ;
    // scale of map
    private float xScale = .95f, yScale = .95f;

    /// <summary> Init vars </summary>
    void Start()
    {
        mainCamera = Camera.main;
        markerHeight = marker.GetComponent<RectTransform>().rect.height;
        maxX = map.maxX;
        minX = map.minX;
        maxZ = map.maxZ;
        minZ = map.minZ;
    }

    /// <summary> Create a marker when the player clicks </summary>
    /// <param name="eventData"> information about the player's click </param>
    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject markerInstant = Instantiate(marker, transform);
        markerInstant.transform.position = eventData.position + Vector2.up * markerHeight;
        MapMarker mapMarker = markerInstant.GetComponent<MapMarker>();
        mapMarker.compass = compass;

        GameObject waypointInstant = Instantiate(waypoint, null);

        Vector2 worldPos = new Vector2((((eventData.position.x - Screen.width/2) * 2 / (Screen.width * xScale)) + 1)/2 * (maxX - minX) + minX, (((eventData.position.y - Screen.height/2) * 2 / (Screen.height * yScale)) + 1)/2 * (maxZ - minZ) + minZ);
        waypointInstant.transform.position = worldPos.x * Vector3.right + worldPos.y * Vector3.forward;

        compass.AddWaypoint(waypointInstant.GetComponent<Waypoint>(), mapMarker);
    }
}
