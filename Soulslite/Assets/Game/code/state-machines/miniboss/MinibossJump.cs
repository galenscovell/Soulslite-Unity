using System.Collections;
using UnityEngine;


public class MinibossJump : StateMachineBehaviour
{
    public string stateName;

    private int hash;
    private Enemy enemy;

    private int[] sfx;

    private Vector2 jumpTarget;
    private Vector2 directionToTarget;
    private bool jumped;
    private bool apex;
    private bool landed;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(Enemy e, int[] assignedSfx)
    {
        hash = Animator.StringToHash(stateName);
        enemy = e;
        sfx = assignedSfx;
    }

    public void SetJumpTarget(Vector2 target)
    {
        jumpTarget = target;
        directionToTarget = (target - enemy.GetBody().position).normalized;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        jumped = false;
        apex = false;
        landed = false;

        enemy.DisableMotion();
        enemy.IgnoreAllCollisions();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.1f && stateTime < 0.2f)
        {
            if (!jumped)
            {
                enemy.EnableMotion();
                enemy.SetMovementImpulse(new Vector2(directionToTarget.x, 1), 5, 0.3f);
                jumped = true;
            }
        }
        else if (stateTime > 0.5f && stateTime < 0.9f)
        {
            enemy.DisableMotion();
        }
        else if (stateTime > 1)
        {
            animator.SetBool("Jumping", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.GetBody().gravityScale = 0;
        enemy.RestoreCollisions();
        enemy.EnableMotion();
    }
}