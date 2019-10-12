using UnityEngine;
using Mirror;

public class PlayerWeapon : NetworkBehaviour
{
    private AudioClip[] audioClipPrefabs;
    private AudioSource audioSource;

    private void Start()
    {
        audioClipPrefabs = GameObject.FindObjectOfType<AudioPrefabsController>().audioClipPrefabs;
        audioSource = GetComponent<AudioSource>();
    }

    [Command]
    public void CmdAddForce(GameObject gameObjectToAddForceTo, Vector3 force)
    {
        Rigidbody rg = gameObjectToAddForceTo.GetComponent<Rigidbody>();
        if (rg != null && !rg.isKinematic) rg.AddForce(force, ForceMode.Impulse);
    }

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

    [ClientRpc]
    public void RpcPlaySound(GameObject weapon, int audioIndex)
    {
        audioSource.PlayOneShot(audioClipPrefabs[audioIndex]);
    }
}
