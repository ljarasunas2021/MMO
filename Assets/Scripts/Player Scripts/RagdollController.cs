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
    #endregion

    #region Initialize
    ///<summary> Initialize components</summary>
    private void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }
    #endregion

    #region BecomeRagdoll
    [Command]
    ///<summary> Become ragdoll </summary>
    public void CmdBecomeRagdoll()
    {
        animator.enabled = false;
        characterController.detectCollisions = false;
        foreach (Rigidbody bodyPart in bodyParts)
        {
            bodyPart.isKinematic = false;
        }
    }
    #endregion
}
