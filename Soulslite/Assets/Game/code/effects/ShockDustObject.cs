using UnityEngine;


public class ShockDustObject : MonoBehaviour
{
    private Animator animator;

    private int playingHash = Animator.StringToHash("Base Layer.Playing");
    private int notPlayingHash = Animator.StringToHash("Base Layer.NotPlaying");


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        int currentState = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;

        if (currentState == notPlayingHash)
        {
            animator.Play(playingHash);
        }
        else
        {
            float playTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (playTime > 1)
            {
                animator.Play(notPlayingHash);
                DustSystem.dustSystem.DespawnShockDust(this);
            }
        }
    }
}

