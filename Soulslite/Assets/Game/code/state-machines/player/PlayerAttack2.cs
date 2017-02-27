using UnityEngine;


public class PlayerAttack2 : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerAttack.Attack2");
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

    public void Chain(Animator animator, int attackVersion)
    {
        animator.SetInteger("AttackVersion", attackVersion);
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.SetNextVelocity(player.GetFacingDirection() * player.GetSpeed());
        vulnerable = true;
        player.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 1f);
        player.SetSpeed(80f);

        DustSystem.dustSystem.SpawnDust(player.GetBody().position, player.GetFacingDirection());
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.2f && stateTime < 1)
        {
            if (player.AbleToMove())
            {
                player.DisableMotion();
                player.RestoreDefaultSpeed();
            }
        }
        else if (stateTime > 1)
        {
            animator.SetInteger("AttackChain", animator.GetInteger("AttackChain") + 1);
            animator.SetInteger("AttackVersion", 1);
            animator.SetBool("Attacking", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.EnableMotion();
    }
}
