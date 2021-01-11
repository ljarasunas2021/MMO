using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MMO.UI.Map
{
    /// <summary> Handles the UI with the map</summary>
    public class Map : MonoBehaviour
    {
        //singleton
        public static Map instance;
        // the map image itself
        public GameObject map;
        // the max and min Z and X values of the player that correspond with the bounds of the map
        public float maxZ, minZ, maxX, minX;
        // map's canvas reference resolution
        [HideInInspector] public Vector2 referenceResolution;
        // map scale
        public Vector2 mapScale = new Vector2(.95f, .95f);
        // the player game object
        [HideInInspector] public GameObject player;
        // the marker that shows the player's current position
        public GameObject playerMarker;
        // the instantiated version of the marker
        private GameObject playerMarkerInstant;
        // the main camera
        private Camera mainCam;
        // the width and height of the map
        [HideInInspector] public float imageWidth, imageHeight;
        // the screen's width and height
        [HideInInspector] public float screenWidth, screenHeight;
        // rect transform component of canvas
        private RectTransform rt;

        
        /// <summary> Init vars </summary>
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.LogError("There already exists an instance of the map script");
            }

            rt = GetComponent<RectTransform>();
            referenceResolution = new Vector2(rt.rect.width, rt.rect.height);

            mainCam = Camera.main;
            Sprite mapSprite = map.GetComponent<Image>().sprite;
            imageWidth = mapSprite.rect.width;
            imageHeight = mapSprite.rect.height;
            screenWidth = Screen.width;
            screenHeight = Screen.height;
        }

        /// <summary> When the map is enabled, refresh the player's position on the marker </summary>
        public void Enable()
        {
            UIManager.instance.LockCursor(false);
            UIManager.instance.canMove = false;
            referenceResolution = new Vector2(rt.rect.width, rt.rect.height);
            Vector3 pos = player.transform.position;
            Vector2 screenPos = new Vector2((pos.x - minX) / (maxX - minX) * referenceResolution.x - referenceResolution.x / 2, (pos.z - minZ) / (maxZ - minZ) * referenceResolution.y - referenceResolution.y / 2);
            Debug.Log(referenceResolution);
            playerMarkerInstant = GameObject.Instantiate(playerMarker, map.transform);
            playerMarkerInstant.transform.localPosition = screenPos;
            playerMarkerInstant.transform.rotation = Quaternion.identity;

            Debug.Log(referenceResolution + " " + new Vector2(screenWidth, screenHeight));
        }

        /// <summary> When the map is disabled, destroy the player's map marker </summary>
        public void Disable()
        {
            UIManager.instance.LockCursor(true);
            UIManager.instance.canMove = true;
            Destroy(playerMarkerInstant);
        }
    }
}
