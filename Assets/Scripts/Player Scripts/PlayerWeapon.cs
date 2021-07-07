using UnityEngine;
using Mirror;

namespace MMO.Player
{
    /// <summary> Executes commands for player's weapon </summary>
    public class PlayerWeapon : NetworkBehaviour
    {
        // audio clips for weapon
        private AudioClip[] audioClipPrefabs;
        // audio source to play sounds from
        private AudioSource audioSource;
        // effects for weapon
        private GameObject[] effectPrefabs;

        /// <summary> Init vars </summary>
        private void Start()
        {
            audioClipPrefabs = GameObject.FindObjectOfType<AudioPrefabsController>().audioClipPrefabs;
            effectPrefabs = GameObject.FindObjectOfType<EffectsPrefabsController>().effectPrefabs;
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary> Add force to gameobject </summary>
        /// <param name="gameObjectToAddForceTo"> add force to this gameobject </param>
        /// <param name="force"> force to add </param>
        [Command]
        public void CmdAddForce(GameObject gameObjectToAddForceTo, Vector3 force)
        {
            Rigidbody rg = gameObjectToAddForceTo.GetComponent<Rigidbody>();
            if (rg != null && !rg.isKinematic) rg.AddForce(force, ForceMode.Impulse);
        }

        /// <summary> Split shells of gun </summary>
        /// <param name="shell"> shell type </param>
        /// <param name="shellSpitPosition"> position for splitting </param>
        /// <param name="shellSpitRotation"> rotation for splitting </param>
        /// <param name="shellSpitForce"> force at which shells will split </param>
        /// <param name="shellForceRandom"> amount of force randomness </param>
        /// <param name="shellSpitTorqueX"> torque to add to the shells in the x direction </param>
        /// <param name="shellTorqueRandom"> amount of torque randomness </param>
        /// <param name="shellSpitTorqueY"> torque to add to the shells in the y direction</param>
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

        /// <summary> Play a sound </summary>
        /// <param name="audioIndex"> index of audio prefab for sound to play </param>
        [ClientRpc]
        public void RpcPlaySound(int audioIndex)
        {
            audioSource.PlayOneShot(audioClipPrefabs[audioIndex]);
        }

        /// <summary> Create a hit effect </summary>
        /// <param name="hitEffectIndex"> index of hit effect in effect prefabs </param>
        /// <param name="hitPoint"> position where hit </param>
        /// <param name="hitNormal"> normal of surface where hit </param>
        [ClientRpc]
        public void RpcCreateHitEffect(int hitEffectIndex, Vector3 hitPoint, Vector3 hitNormal)
        {
            Instantiate(effectPrefabs[hitEffectIndex], hitPoint, Quaternion.FromToRotation(Vector3.up, hitNormal));
        }

        /// <summary> Create a muzzle effect </summary>
        /// <param name="muzfxIndex"> index of muzzle effect in effect prefabs </param>
        /// <param name="position"> position for muzzle effect </param>
        /// <param name="rotation"> rotation for muzzle effect </param>
        [ClientRpc]
        public void RpcMakeMuzzleEffect(int muzfxIndex, Vector3 position, Vector3 rotation)
        {
            Instantiate(effectPrefabs[muzfxIndex], position, Quaternion.Euler(rotation));
        }
    }
}
