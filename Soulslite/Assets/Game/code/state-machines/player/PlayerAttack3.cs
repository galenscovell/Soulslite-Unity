using UnityEngine;


public class PlayerAttack3 : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerAttack.Attack3");
    private int interruptHash = Animator.StringToHash("Base Layer.PlayerAttackInterrupt");
    private PlayerAgent player;
    private int sfxIndex;

    // Denotes when this state can be interrupted
    private bool vulnerable = true;
    private bool interrupted;


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
            interrupted = true;
            return true;
        }
        return false;
    }

    public bool IsVulnerable()
    {
        return vulnerable;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        interrupted = false;
        vulnerable = true;

        player.SetNextVelocity(player.GetFacingDirection() * player.GetSpeed());
        player.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 1f);
        player.SetSpeed(80f);

        DustSystem.dustSystem.SpawnDust(player.GetBody().position, player.GetFacingDirection());
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.25f && stateTime < 1)
        {
            if (player.AbleToMove())
            {
                player.DisableMotion();
                player.RestoreDefaultSpeed();
            }

            if (interrupted)
            {
                animator.SetInteger("AttackVersion", 1);
                animator.SetBool("Attacking", false);
            }
        }
        else if (stateTime > 1)
        {
            animator.SetInteger("AttackVersion", 1);
            animator.SetBool("Attacking", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (interrupted)
        {
            animator.Play(interruptHash);
        }
        else
        {
            player.EnableMotion();
        }
    }
}
