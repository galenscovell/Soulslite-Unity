using UnityEngine;


public class PlayerFall : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerFall.PlayerFall");
    private PlayerAgent player;
    private int sfxIndex;

    private bool descentBegan;
    private bool fadeOutBegan;
    private bool sfxPlayed;


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
        descentBegan = false;
        fadeOutBegan = false;
        sfxPlayed = false;

        player.SetNextVelocity(new Vector2(player.GetFacingDirection().x * 0.001f, -8));
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float stateTime = stateInfo.normalizedTime;

        if (stateTime < 0.1f)
        {
            if (!descentBegan)
            {
                player.LerpSpeed(player.GetDefaultSpeed(), 300, 0.5f);
                descentBegan = true;
            }
        }
        else if (stateTime > 0.5f && stateTime < 1)
        {
            if (!fadeOutBegan)
            {
                player.FadeOutSprite(0.5f);
                fadeOutBegan = true;
            }
        }
        else if (stateTime > 2 && stateTime < 7)
        {
            if (!sfxPlayed)
            {
                player.PlaySfx(sfxIndex, 1f, 1f);
                sfxPlayed = true;
            }
        }
        else if (stateTime > 7)
        {
            animator.SetTrigger("Revive");
        }

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.Revive();
    }
}
