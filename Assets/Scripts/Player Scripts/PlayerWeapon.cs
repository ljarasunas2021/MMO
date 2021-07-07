using UnityEngine;
using Mirror;

///<summary> Used to do weapon actions that have to be done on the network </summary>
public class PlayerWeapon : NetworkBehaviour
{
    #region Variables
    public GameObject nonRagdoll;
    // array of audio clip prefabs
    private AudioClip[] audioClipPrefabs;
    // audio source on gameObject
    private AudioSource audioSource;
    // array of effect prefabs
    private GameObject[] effectPrefabs;
    #endregion

    #region Initialize
    ///<summary> Initialize variables </summary>
    private void Start()
    {
        audioClipPrefabs = GameObject.FindObjectOfType<AudioPrefabsController>().audioClipPrefabs;
        effectPrefabs = GameObject.FindObjectOfType<EffectsPrefabsController>().effectPrefabs;
        audioSource = nonRagdoll.GetComponent<AudioSource>();
    }
    #endregion

    #region NetworkedVoids
    [Command]
    ///<summary> Add force to gameObject </summary>
    public void CmdAddForce(GameObject gameObjectToAddForceTo, Vector3 force)
    {
        Rigidbody rg = gameObjectToAddForceTo.GetComponent<Rigidbody>();
        if (rg != null && !rg.isKinematic) rg.AddForce(force, ForceMode.Impulse);
    }

    [Command]
    ///<summary> Split shells </summary>
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
    ///<summary> Play sound on all clients </summary>
    public void RpcPlaySound(int audioIndex)
    {
        audioSource.PlayOneShot(audioClipPrefabs[audioIndex]);
    }

    [ClientRpc]
    ///<summary> Create hit effect on all clients </summary>
    public void RpcCreateHitEffect(int hitEffectIndex, Vector3 hitPoint, Vector3 hitNormal)
    {
        Instantiate(effectPrefabs[hitEffectIndex], hitPoint, Quaternion.FromToRotation(Vector3.up, hitNormal));
    }

    [ClientRpc]
    ///<summary> Create a muzzle effect </summary>
    public void RpcMakeMuzzleEffect(int muzfxIndex, Vector3 position, Vector3 rotation)
    {
        Instantiate(effectPrefabs[muzfxIndex], position, Quaternion.Euler(rotation));
    }
    #endregion
}
