﻿using UnityEngine;


public class EnemyRangedHurt : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.EnemyRangedHurt");
    private Enemy enemy;
    private Vector2 flungVelocity;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(Enemy e)
    {
        enemy = e;
    }

    public void SetFlungVelocity(Vector2 velocity)
    {
        flungVelocity = velocity;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.EnableFlippedX();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;
        if (stateTime < 0.325f)
        {
            enemy.SetSpeed(180f);
            enemy.SetNextVelocity(flungVelocity);
        }
        else if (stateTime >= 0.325f && stateTime < 1)
        {
            enemy.SetSpeed(enemy.defaultSpeed);
            enemy.DisableMotion();
        }
        else if (stateTime >= 1)
        {
            enemy.EnableDeath();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.EnableMotion();
    }
}
