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
        player.DisableMotion();
        player.PlaySfx(sfxIndex, 1f, 1f);

        animator.SetBool("Attacking", false);
        animator.SetBool("Dashing", false);
        animator.SetBool("Idling", false);
        animator.SetBool("Ranged", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;
        if (stateTime >= 2)
        {
            if (!fadeOutBegan)
            {
                LeanTween.value(player.gameObject, player.SetSpriteAlpha, 1, 0, 1);
                fadeOutBegan = true;
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
