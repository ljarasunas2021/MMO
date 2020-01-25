using UnityEngine.EventSystems;
using UnityEngine;

public class MapMarker : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public Compass compass;

    public void OnPointerClick(PointerEventData eventData)
    {
        compass.RemoveWaypoint(this);
    }
}
