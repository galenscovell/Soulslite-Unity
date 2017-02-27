using UnityEngine;


public class PlayerAttackInterrupt : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerAttackInterrupt");
    private PlayerAgent player;
    private int sfxIndex;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(PlayerAgent playerEntity, int assignedSfxIndex)
    {
        player = playerEntity;
        sfxIndex = assignedSfxIndex;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Attacking", false);
        animator.SetInteger("AttackChain", 0);

        int currentAttackVersion = animator.GetInteger("AttackVersion");
        if (currentAttackVersion == 1)
        {
            animator.SetInteger("AttackVersion", 2);
        }
        else if (currentAttackVersion == 2)
        {
            animator.SetInteger("AttackVersion", 1);
        }
        else if (currentAttackVersion == 3)
        {
            animator.SetInteger("AttackVersion", 1);
        }

        player.DisableMotion();
        player.PlaySfx(sfxIndex, 1f, 0.25f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.EnableMotion();
    }
}
