using UnityEngine.EventSystems;
using UnityEngine;

namespace MMO.UI.Maps
{
    /// <summary> Markers on the map </summary>
    public class MapMarker : MonoBehaviour, IPointerClickHandler
    {
        // the compass gameObject
        [HideInInspector] public Compass compass;

        /// <summary> Destroy the gameObject and remove it from the compass on click </summary>
        /// <param name="eventData"> information of the pointer's click </param>
        public void OnPointerClick(PointerEventData eventData)
        {
            compass.RemoveWaypoint(this);
        }
    }
}
