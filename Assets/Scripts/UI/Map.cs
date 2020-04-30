﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    // the map image itself
    public GameObject map;
    // the max and min Z and X values of the player that correspond with the bounds of the map
    public float maxZ, minZ, maxX, minX;
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

    // initialize the variables at runtime
    void Awake()
    {
        mainCam = Camera.main;
        Sprite mapSprite = map.GetComponent<Image>().sprite;
        imageWidth = mapSprite.rect.width;
        imageHeight = mapSprite.rect.height;
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    // Called when map becomes visible, spawns player marker
    public void Enable()
    {
        Debug.Log(player.gameObject.name);
        UIManager.LockCursor(false);
        UIManager.canMove = false;
        Vector3 pos = player.transform.position;
        Vector2 screenPos = new Vector2((imageWidth) * (pos.x - minX) / (maxX - minX), (imageHeight) * (pos.z - minZ) / (maxZ - minZ));
        playerMarkerInstant = GameObject.Instantiate(playerMarker, map.transform);
        playerMarkerInstant.transform.localPosition = screenPos - new Vector2(imageWidth / 2, imageHeight / 2);
        playerMarkerInstant.transform.rotation = Quaternion.identity;
    }

    // called when map becomes invisible, deletes player marker
    public void Disable()
    {
        UIManager.LockCursor(true);
        UIManager.canMove = true;
        Destroy(playerMarkerInstant);
    }
}