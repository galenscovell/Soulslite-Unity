using UnityEngine;


public class PlayerDeath : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerDeath");
    private PlayerAgent player;
    private int sfxIndex;

    private bool fadeOutBegan = false;


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
        player.PlaySfx(sfxIndex, 1f, 1f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;
        if (stateTime > 2 && stateTime < 3)
        {
            animator.SetBool("Dead", true);
            if (!fadeOutBegan)
            {
                player.FadeOutSprite(0.5f);
                fadeOutBegan = true;
            }
        }
        else if (stateTime > 3)
        {
            animator.SetTrigger("Revive");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.Revive();
    }
}
