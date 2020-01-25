using UnityEngine.EventSystems;
using UnityEngine;

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
    // the scale of the canvas
    private float localScale;
    // the screen's width and height
    private float screenWidth, screenHeight;
    // the height of the marker
    private float imageHeight;
    // the map's height and width
    private float mapWidth, mapHeight;
    //the max coordinates of the player on the mapHeight
    private float maxX, minX, maxZ, minZ;

    // initialize the variables
    void Start()
    {
        mainCamera = Camera.main;
        localScale = GetComponent<RectTransform>().localScale.x;
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        imageHeight = marker.GetComponent<RectTransform>().rect.height;
        mapWidth = map.imageWidth;
        mapHeight = map.imageHeight;
        maxX = map.maxX;
        minX = map.minX;
        maxZ = map.maxZ;
        minZ = map.minZ;
    }

    // create a marker on click
    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject markerInstant = Instantiate(marker, transform);
        Vector2 localPos = (eventData.position - new Vector2(screenWidth, screenHeight) / 2) / localScale + Vector2.up * imageHeight / 2;
        markerInstant.transform.localPosition = localPos;
        MapMarker mapMarker = markerInstant.GetComponent<MapMarker>();
        mapMarker.compass = compass;

        GameObject waypointInstant = Instantiate(waypoint, null);

        Vector2 screenPos = localPos + Vector2.down * imageHeight / 2 + new Vector2(mapWidth / 2, mapHeight / 2);
        Vector2 worldPos = new Vector2((screenPos.x * (maxX - minX) / mapWidth) + minX, (screenPos.y * (maxZ - minZ) / mapHeight) + minZ);
        waypointInstant.transform.position = worldPos.x * Vector3.right + worldPos.y * Vector3.forward;

        compass.AddWaypoint(waypointInstant.GetComponent<Waypoint>(), mapMarker);
    }
}
