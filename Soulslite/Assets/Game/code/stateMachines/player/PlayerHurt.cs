using UnityEngine;


public class PlayerHurt : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerHurt");
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
        animator.SetBool("FullIdle", false);
        player.DisableMotion();
        player.PlaySfx(sfxIndex, 1f, 1f);
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
        player.EnableMotion();
    }
}
