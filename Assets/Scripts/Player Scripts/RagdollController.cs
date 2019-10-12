using UnityEngine;
using Mirror;

///<summary> controls the player becoming a ragdoll </summary>
public class RagdollController : NetworkBehaviour
{
    #region Initialize
    // array of the player's bodyparts
    public Rigidbody[] bodyParts;
    // animator of player
    private Animator animator;
    // character controller of player
    private CharacterController characterController;
    // array of the network children on the player go
    private NetworkTransformChild[] ntChildren;
    #endregion

    #region Initialize
    ///<summary> Initialize components</summary>
    private void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        ntChildren = transform.GetComponents<NetworkTransformChild>();
    }
    #endregion

    #region BecomeRagdoll
    [Command]
    ///<summary> Become ragdoll </summary>
    public void CmdBecomeRagdoll()
    {
        RpcBecomeRagdoll();
        animator.enabled = false;
        characterController.detectCollisions = false;
        foreach (Rigidbody bodyPart in bodyParts) bodyPart.isKinematic = false;
        EnableNTS();
    }

    [ClientRpc]
    ///<summary> Make sure there is no anim on the player </summary>
    private void RpcBecomeRagdoll()
    {
        animator.enabled = false;
        characterController.detectCollisions = false;
        EnableNTS();
    }

    private void EnableNTS() { foreach (NetworkTransformChild child in ntChildren) child.enabled = true; }
    #endregion
}
