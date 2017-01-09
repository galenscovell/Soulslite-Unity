using System.Collections;
using UnityEngine;


public class PlayerMovementState : StateMachineBehaviour
{
    private PlayerAgent player;
    private AudioSource footstepSound;
    private IEnumerator coroutine;


    public void Setup(PlayerAgent playerEntity, AudioSource sound)
    {
        player = playerEntity;
        footstepSound = sound;
        coroutine = Footsteps();
    }

    private IEnumerator Footsteps()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.225f);
            footstepSound.pitch = Random.Range(0.9f, 1.2f);
            footstepSound.PlayOneShot(footstepSound.clip);
            yield return new WaitForSeconds(0.225f);
        }
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.StartCoroutine(coroutine);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.StopCoroutine(coroutine);
    }
}
