using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRoyalePlayer : MonoBehaviour
{
    [HideInInspector] public bool dropped = false;
    private SchoolBus bus;
    private PlayerCameraManager playerCameraManager;
    public GameObject nonRagdoll;

    private void Start()
    {
        bus = GameObject.FindObjectOfType<SchoolBus>();
        playerCameraManager = GetComponent<PlayerCameraManager>();
    }

    public void Drop()
    {
        dropped = true;
        CharacterController cc = nonRagdoll.GetComponent<CharacterController>();
        cc.enabled = false;
        nonRagdoll.transform.position = bus.dropPos.position;
        cc.enabled = true;
        playerCameraManager.ChangeCam(CameraModes.cinematic);
    }
}
