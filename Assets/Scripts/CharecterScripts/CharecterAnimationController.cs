using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class CharecterAnimationController : NetworkBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    //this only works if server is also a client as ServerRPC command just plays the animation
    //it does not call a clientRPC command
    public void NetworkPlayAnimation(string animationName)
    {
        if (IsServer)
        {
            PlayAnimationClientRPC(animationName);
        }
        else
        {
            PlayAnimationServerRPC(animationName);
            animator.Play(animationName);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayAnimationServerRPC(string animationName)
    {
        animator.Play(animationName);
    }

    [ClientRpc]
    private void PlayAnimationClientRPC(string animationName)
    {
        animator.Play(animationName);
    }

    public void PlayAnimation(string animationName)
    {
        animator.Play(animationName);
    }
}
