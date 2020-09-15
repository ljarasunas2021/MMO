using UnityEngine.EventSystems;
using UnityEngine;

// on maps marker
public class MapMarker : MonoBehaviour, IPointerClickHandler
{
    // the compass gameObject
    [HideInInspector] public Compass compass;

    // Destroy the gameObject and remove it form the compass on click
    public void OnPointerClick(PointerEventData eventData)
    {
        compass.RemoveWaypoint(this);
    }
}
