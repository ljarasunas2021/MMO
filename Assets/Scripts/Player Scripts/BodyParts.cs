using UnityEngine;
using Mirror;

namespace MMO.Player
{
    /// <summary> Holds variables for player's body parts</summary>
    public class BodyParts : NetworkBehaviour
    {
        // body part gameobjects
        public GameObject handR;
        public GameObject head;
        public GameObject lockedCamFollow;
    }
}
