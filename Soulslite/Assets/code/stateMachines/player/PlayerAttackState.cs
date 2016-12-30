using UnityEngine;


public class PlayerAttackState : StateMachineBehaviour
{
    private PlayerAgent player;


    public void Setup(PlayerAgent playerEntity)
    {
        player = playerEntity;
    }

    public void Interrupt(Animator animator)
    {
        player.nextVelocity = Vector2.zero;
        animator.SetTrigger("Attack");
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.nextVelocity = player.facingDirection * player.speedMultiplier;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime >= 0.2f)
        {
            player.canMove = false;
        }

        if (stateTime >= 1)
        {
            animator.SetTrigger("Attack");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.canMove = true;
    }
}
