using UnityEngine;


public class PlayerDashState : StateMachineBehaviour
{
    private PlayerAgent player;
    private DashTrail dashTrail;

    public float dashSpeed;


    public void Setup(PlayerAgent playerEntity, DashTrail trail)
    {
        player = playerEntity;
        dashTrail = trail;
    }

    public void Interrupt(Animator animator)
    {
        dashTrail.SetEnabled(false);
        player.nextVelocity = Vector2.zero;
        animator.SetBool("Dashing", false);
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        dashTrail.SetEnabled(true);
        player.speedMultiplier = dashSpeed;
        player.nextVelocity = player.facingDirection * player.speedMultiplier;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime >= 0.2f)
        {
            dashTrail.SetEnabled(false);
            player.canMove = false;
        }

        if (stateTime >= 1)
        {
            animator.SetBool("Dashing", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.canMove = true;
    }
}
