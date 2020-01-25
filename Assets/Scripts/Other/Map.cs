using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public GameObject map;
    public float maxZ, minZ, maxX, minX;
    [HideInInspector] public GameObject player;
    public GameObject playerMarker;
    private GameObject playerMarkerInstant;
    private Camera mainCam;
    [HideInInspector] public float imageWidth, imageHeight, screenWidth, screenHeight;

    void Awake()
    {
        mainCam = Camera.main;
        Sprite mapSprite = map.GetComponent<Image>().sprite;
        imageWidth = mapSprite.rect.width;
        imageHeight = mapSprite.rect.height;

        // imageWidth = map.GetComponent<RectTransform>().rect.width;
        // imageHeight = map.GetComponent<RectTransform>().rect.height;
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        Debug.Log(screenWidth + " " + screenHeight + " " + imageWidth + " " + imageHeight);
    }

    public void Enable()
    {
        UIManager.LockCursor(false);
        UIManager.canMove = false;
        Vector3 pos = player.transform.position;
        Vector2 screenPos = new Vector2((imageWidth) * (pos.x - minX) / (maxX - minX), (imageHeight) * (pos.z - minZ) / (maxZ - minZ));
        playerMarkerInstant = GameObject.Instantiate(playerMarker, map.transform);
        playerMarkerInstant.transform.localPosition = screenPos - new Vector2(imageWidth / 2, imageHeight / 2);
        playerMarkerInstant.transform.rotation = Quaternion.identity;
    }

    public void Disable()
    {
        UIManager.LockCursor(true);
        UIManager.canMove = true;
        Destroy(playerMarkerInstant);
    }
}
