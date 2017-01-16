﻿using UnityEngine;


public class PlayerAttack3 : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerAttack.PlayerAttack3");
    private PlayerAgent player;
    private int sfxIndex;

    // Denotes when this state can be interrupted
    private bool vulnerable = true;


    public int GetHash()
    {
        return hash;
    }

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

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.SetNextVelocity(player.facingDirection * player.GetSpeed());
        vulnerable = true;
        player.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 1f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;
        if (stateTime < 0.25f)
        {
            player.SetNextVelocity(player.facingDirection * player.GetSpeed());
        }
        else if (player.AbleToMove() && stateTime >= 0.25f)
        {
            player.DisableMotion();
            player.SetSpeed(player.GetDefaultSpeed());
        }
        else if (stateTime >= 1)
        {
            animator.SetInteger("AttackVersion", 1);
            animator.SetBool("Attacking", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.EnableMotion();
    }
}