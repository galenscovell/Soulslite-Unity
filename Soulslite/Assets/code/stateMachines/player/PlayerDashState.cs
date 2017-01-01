using UnityEngine;


public class PlayerDashState : StateMachineBehaviour
{
    private PlayerAgent player;
    private DashTrail dashTrail;
    private float dashSpeed = 1200f;


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
        player.SetSpeed(dashSpeed);
        player.nextVelocity = player.facingDirection * player.GetSpeed();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;
        if (player.AbleToMove() && stateTime >= 0.15f)
        {
            dashTrail.SetEnabled(false);
            player.DisableMotion();
            player.SetSpeed(player.GetNormalSpeed());
        }
        else if (stateTime >= 1)
        {
            animator.SetBool("Dashing", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.EnableMotion();
    }
}
