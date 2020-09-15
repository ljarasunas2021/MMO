using UnityEngine;
using Mirror;

// does commands for player's weapon
public class PlayerWeapon : NetworkBehaviour
{
    // audio clips for weapon
    private AudioClip[] audioClipPrefabs;
    // audio source to play sounds from
    private AudioSource audioSource;
    // effects for weapon
    private GameObject[] effectPrefabs;

    // init vars
    private void Start()
    {
        audioClipPrefabs = GameObject.FindObjectOfType<AudioPrefabsController>().audioClipPrefabs;
        effectPrefabs = GameObject.FindObjectOfType<EffectsPrefabsController>().effectPrefabs;
        audioSource = GetComponent<AudioSource>();
    }

    // add force to gameobject
    [Command]
    public void CmdAddForce(GameObject gameObjectToAddForceTo, Vector3 force)
    {
        Rigidbody rg = gameObjectToAddForceTo.GetComponent<Rigidbody>();
        if (rg != null && !rg.isKinematic) rg.AddForce(force, ForceMode.Impulse);
    }

    // split shells of gun
    [Command]
    public void CmdSplitShells(GameObject shell, Vector3 shellSpitPosition, Vector3 shellSpitRotation, float shellSpitForce, float shellForceRandom, float shellSpitTorqueX, float shellTorqueRandom, float shellSpitTorqueY)
    {
        GameObject shellGO = Instantiate(shell, shellSpitPosition, Quaternion.Euler(shellSpitRotation));
        NetworkServer.Spawn(shellGO);
        Rigidbody shellRG = shellGO.GetComponent<Rigidbody>();
        shellRG.isKinematic = false;
        shellRG.AddForce(new Vector3(shellSpitForce + Random.Range(0, shellForceRandom), 0, 0), ForceMode.Impulse);
        shellRG.AddTorque(new Vector3(shellSpitTorqueX + Random.Range(-shellTorqueRandom, shellTorqueRandom), shellSpitTorqueY + Random.Range(-shellTorqueRandom, shellTorqueRandom), 0), ForceMode.Impulse);
    }

    // play sound
    [ClientRpc]
    public void RpcPlaySound(int audioIndex)
    {
        audioSource.PlayOneShot(audioClipPrefabs[audioIndex]);
    }

    // create hit effect
    [ClientRpc]
    public void RpcCreateHitEffect(int hitEffectIndex, Vector3 hitPoint, Vector3 hitNormal)
    {
        Instantiate(effectPrefabs[hitEffectIndex], hitPoint, Quaternion.FromToRotation(Vector3.up, hitNormal));
    }

    // create muzzle effect
    [ClientRpc]
    public void RpcMakeMuzzleEffect(int muzfxIndex, Vector3 position, Vector3 rotation)
    {
        Instantiate(effectPrefabs[muzfxIndex], position, Quaternion.Euler(rotation));
    }
}
