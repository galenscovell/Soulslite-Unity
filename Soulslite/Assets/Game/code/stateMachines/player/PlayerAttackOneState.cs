using UnityEngine;


public class PlayerAttackOneState : StateMachineBehaviour
{
    private PlayerAgent player;
    private AudioSource attackSound;

    // Denotes when this state can be interrupted
    private bool vulnerable = true;


    public void Setup(PlayerAgent playerEntity, AudioSource sound)
    {
        player = playerEntity;
        attackSound = sound;
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
        attackSound.PlayOneShot(attackSound.clip);
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
            player.SetSpeed(player.GetNormalSpeed());
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