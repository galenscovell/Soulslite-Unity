using System.Collections;
using UnityEngine;


public class PlayerMovement : StateMachineBehaviour
{
    private int hash = Animator.StringToHash("Base Layer.PlayerMovement");
    private PlayerAgent player;
    private int sfxIndex;
    private IEnumerator footstep;


    public int GetHash()
    {
        return hash;
    }

    public void Setup(PlayerAgent playerEntity, int assignedSfxIndex)
    {
        player = playerEntity;
        sfxIndex = assignedSfxIndex;
        footstep = Footsteps();
    }

    private IEnumerator Footsteps()
    {
        while (true)
        {
            DustSystem.dustSystem.SpawnDust(player.GetBody().position, player.GetFacingDirection());
            yield return new WaitForSeconds(0.21f);
            player.PlaySfxRandomPitch(sfxIndex, 0.9f, 1.2f, 0.4f);
            yield return new WaitForSeconds(0.21f);
        }
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.StartCoroutine(footstep);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.StopCoroutine(footstep);
    }
}
