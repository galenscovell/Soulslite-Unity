using System.Collections;
using UnityEngine;


public class PlayerMovement : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerMovement");
    private PlayerAgent player;
    private int sfxIndex;
    private IEnumerator coroutine;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(PlayerAgent playerEntity, int assignedSfxIndex)
    {
        player = playerEntity;
        sfxIndex = assignedSfxIndex;
        coroutine = Footsteps();
    }

    private IEnumerator Footsteps()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.225f);
            player.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.2f, 1f);
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
