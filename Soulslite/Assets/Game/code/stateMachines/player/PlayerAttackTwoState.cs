﻿using UnityEngine;


public class PlayerAttackTwoState : StateMachineBehaviour
{
    private PlayerAgent player;

    // Denotes when this state can be interrupted
    private bool vulnerable = true;

    private int sfxIndex;


    public void Setup(PlayerAgent playerEntity, int assignedSfxIndex)
    {
        player = playerEntity;
        sfxIndex = assignedSfxIndex;
    }

    public bool Interrupt(Animator animator)
    {
        if (vulnerable)
        {
            animator.SetInteger("AttackVersion", 1);
            animator.SetBool("Attacking", false);
            return true;
        }
        return false;
    }

    public void Chain(Animator animator, int attackVersion)
    {
        animator.SetInteger("AttackVersion", attackVersion);
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.SetNextVelocity(player.facingDirection * player.GetSpeed());
        vulnerable = true;
        player.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 0.75f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;
        if (stateTime < 0.2f)
        {
            player.SetNextVelocity(player.facingDirection * player.GetSpeed());
        }
        else if (player.AbleToMove() && stateTime >= 0.2f)
        {
            player.DisableMotion();
            player.SetSpeed(player.GetDefaultSpeed());
        }
        else if (stateTime >= 1)
        {
            player.ChainAttack();
            animator.SetInteger("AttackVersion", 1);
            animator.SetBool("Attacking", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.EnableMotion();
    }
}
