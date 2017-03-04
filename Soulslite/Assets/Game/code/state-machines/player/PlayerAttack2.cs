using UnityEngine;


public class PlayerAttack2 : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerAttack.Attack2");
    private int interruptHash = Animator.StringToHash("Base Layer.PlayerAttackInterrupt");
    private PlayerAgent player;
    private int sfxIndex;

    // Denotes when this state can be interrupted
    private bool vulnerable = true;
    private bool moved;
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

    public void Chain(Animator animator, int attackVersion)
    {
        animator.SetInteger("AttackVersion", attackVersion);
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        moved = false;
        interrupted = false;
        vulnerable = true;

        player.DisableMotion();
        player.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.3f, 1f);
        DustSystem.dustSystem.SpawnDust(player.GetBody().position, player.GetFacingDirection());
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime > 0.1f && stateTime < 1)
        {
            if (!moved)
            {
                player.SetMovementImpulse(player.GetFacingDirection().normalized, 1f, 0.075f);
                DustSystem.dustSystem.SpawnDust(player.GetBody().position, player.GetFacingDirection());
                moved = true;
            }

            if (interrupted)
            {
                player.SetMovementImpulse(Vector2.zero, 0, 0);
                animator.SetInteger("AttackVersion", 1);
                animator.SetBool("Attacking", false);
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
