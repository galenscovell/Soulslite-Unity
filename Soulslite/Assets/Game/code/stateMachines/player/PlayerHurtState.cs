using UnityEngine;


public class PlayerHurtState : StateMachineBehaviour
{
    private PlayerAgent player;
    private AudioSource hurtSound;


    public void Setup(PlayerAgent playerEntity, AudioSource sound)
    {
        player = playerEntity;
        hurtSound = sound;
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.DisableMotion();
        hurtSound.PlayOneShot(hurtSound.clip);
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
