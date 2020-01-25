using UnityEngine.EventSystems;
using UnityEngine;

public class MapMarkers : MonoBehaviour, IPointerClickHandler
{
    public Compass compass;
    public Map map;
    public GameObject marker, waypoint;
    private Camera mainCamera;
    private float localScale, screenWidth, screenHeight, imageHeight, mapWidth, mapHeight, maxX, minX, maxZ, minZ;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject markerInstant = Instantiate(marker, transform);
        Vector2 localPos = (eventData.position - new Vector2(screenWidth, screenHeight) / 2) / localScale + Vector2.up * imageHeight / 2;
        markerInstant.transform.localPosition = localPos;
        GameObject waypointInstant = Instantiate(waypoint, null);

        Vector2 screenPos = localPos + Vector2.down * imageHeight / 2 + new Vector2(mapWidth / 2, mapHeight / 2);
        Vector2 worldPos = new Vector2((screenPos.x * (maxX - minX) / mapWidth) + minX, (screenPos.y * (maxZ - minZ) / mapHeight) + minZ);
        waypointInstant.transform.position = worldPos.x * Vector3.right + worldPos.y * Vector3.forward;

        compass.AddWaypoint(waypointInstant.GetComponent<Waypoint>());
    }
}
