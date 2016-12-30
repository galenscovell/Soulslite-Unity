using UnityEngine;


public class PlayerHurtState : StateMachineBehaviour
{
    private PlayerAgent player;


    public void Setup(PlayerAgent playerEntity)
    {
        player = playerEntity;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.canMove = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime >= 1)
        {
            animator.SetTrigger("Hurt");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.canMove = true;
    }
}
