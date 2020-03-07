using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BattleRoyalePlayer : NetworkBehaviour
{
    [HideInInspector] public bool dropped = false;
    private SchoolBus bus;
    private PlayerCameraManager playerCameraManager;
    private MinimapCamera minimapCamera;

    private void Start()
    {
        bus = GameObject.FindObjectOfType<SchoolBus>();
        playerCameraManager = GetComponent<PlayerCameraManager>();
        minimapCamera = GetComponentInChildren<MinimapCamera>();
    }

    public void Drop()
    {
        dropped = true;
        CharacterController cc = GetComponent<CharacterController>();
        cc.enabled = false;
        transform.position = bus.dropPos.position;
        cc.enabled = true;
        playerCameraManager.ChangeCam(CameraModes.cinematic);
        minimapCamera.ChangeParent(transform);
    }
}
